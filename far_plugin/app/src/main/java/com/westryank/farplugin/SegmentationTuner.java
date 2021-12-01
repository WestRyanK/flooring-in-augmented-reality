package com.westryank.farplugin;

import android.graphics.Bitmap;
import android.util.Log;

import org.opencv.android.Utils;
import org.opencv.core.Core;
import org.opencv.core.CvType;
import org.opencv.core.Mat;
import org.opencv.core.Point;
import org.opencv.core.Scalar;
import org.opencv.core.Size;
import org.opencv.imgproc.Imgproc;

import java.util.ArrayList;
import java.util.List;

public class SegmentationTuner {
    private static final String TAG = "Far:SegmentationTuner";
    private static final Scalar FOREGROUND = new Scalar(1);
    private static final Scalar BACKGROUND = new Scalar(2);
    private static Mat MORPHOLOGY_KERNEL;
    private static Point KERNEL_ANCHOR;

    private OutputResultTypeEnum _outputResultType;
    private float _intermediateSize = 1.0f;

    private Mat maskMat;
    private Mat imageMat;
    private Mat watershedMaskMat;
    private Mat sureForegroundMaskMat;
    private Mat sureBackgroundMaskMat;

    public SegmentationTuner(OutputResultTypeEnum inOutputResultType) {
        OpenCVUtils.InitializeOpenCV();

        MORPHOLOGY_KERNEL = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(3, 3));
        KERNEL_ANCHOR = new Point(-1, -1);
        SetOutputResultType(inOutputResultType);
        maskMat = new Mat();
        imageMat = new Mat();
        watershedMaskMat = new Mat();
        sureForegroundMaskMat = new Mat();
        sureBackgroundMaskMat = new Mat();
    }

    public void Dispose() {
        if (MORPHOLOGY_KERNEL != null) {
            MORPHOLOGY_KERNEL.release();
        }
    }

    public Bitmap ConvertMaskBitmap(Bitmap inMask) {
        BitmapToMaskMat(inMask);
        Core.absdiff(maskMat, new Scalar(1), maskMat);
        Imgproc.cvtColor(maskMat, maskMat, Imgproc.COLOR_GRAY2BGRA);
        Core.multiply(maskMat, new Scalar(255, 255, 255, 1), maskMat);
        Core.rotate(maskMat, maskMat, Core.ROTATE_180);
        Bitmap outputBitmap = Bitmap.createBitmap(inMask.getWidth(), inMask.getHeight(), Bitmap.Config.ARGB_8888);
        Utils.matToBitmap(maskMat, outputBitmap);
        return outputBitmap;
    }

    private void BitmapToMaskMat(Bitmap inMask) {
        Utils.bitmapToMat(inMask, maskMat);
        Core.extractChannel(maskMat, maskMat, 3);
        Imgproc.threshold(maskMat, maskMat, 0, 1, Imgproc.THRESH_BINARY);
    }

    private void BitmapToImageMat(Bitmap inImage) {
        Utils.bitmapToMat(inImage, imageMat);
        Imgproc.cvtColor(imageMat, imageMat, Imgproc.COLOR_BGRA2BGR);
    }

    public Bitmap Finetune(Bitmap inInputBitmap, Bitmap inMask) {
        if (inInputBitmap == null) {
            Log.e(TAG, "Finetune: inInputBitmap is null");
            return null;
        }

        if (inMask == null) {
            Log.e(TAG, "Finetune: inMask is null");
            return null;
        }

        // TODO: Optimize so that all these mats are reused
        BitmapToMaskMat(inMask);
        BitmapToImageMat(inInputBitmap);
        Size originalSize = maskMat.size();
        if (_intermediateSize != 1.0f) {
            Imgproc.resize(maskMat, maskMat, new Size(), _intermediateSize, _intermediateSize, Imgproc.INTER_LINEAR);
            Imgproc.resize(imageMat, imageMat, new Size(), _intermediateSize, _intermediateSize, Imgproc.INTER_LINEAR);
        }

        Mat segmentationMat = WatershedSegmentation(imageMat, maskMat);
        Imgproc.cvtColor(segmentationMat, segmentationMat, Imgproc.COLOR_GRAY2BGRA);
        if (_outputResultType == OutputResultTypeEnum.Final) {
            Core.multiply(segmentationMat, new Scalar(255, 255, 255, 1), segmentationMat);
        } else if (_outputResultType == OutputResultTypeEnum.SegmentationMask) {
            Core.multiply(segmentationMat, new Scalar(127, 127, 127, 1), segmentationMat);
        }

        if (_intermediateSize != 1.0f) {
            Imgproc.resize(segmentationMat, segmentationMat, originalSize, 0, 0, Imgproc.INTER_NEAREST);
        }
        Core.rotate(segmentationMat, segmentationMat, Core.ROTATE_180);
        // TODO: Optimize so we don't have to create this bitmap each time.
        Bitmap outputBitmap = Bitmap.createBitmap(inInputBitmap.getWidth(), inInputBitmap.getHeight(), Bitmap.Config.ARGB_8888);
        Utils.matToBitmap(segmentationMat, outputBitmap);

        Log.d(TAG, "OutputBmp: " + Integer.toString(outputBitmap.getWidth()) + " " + Integer.toString(outputBitmap.getHeight()) + " " + outputBitmap.getConfig().toString());

        return outputBitmap;
    }

    private void CreateWatershedMask(Mat inApproxForegroundMaskMat) {
        int erode_size = (int) (21 * _intermediateSize); // 21
        Imgproc.erode(inApproxForegroundMaskMat, sureForegroundMaskMat, MORPHOLOGY_KERNEL, KERNEL_ANCHOR, erode_size);

        int dilate_size = (int) (21 * _intermediateSize); // 41
        Imgproc.dilate(inApproxForegroundMaskMat, sureBackgroundMaskMat, MORPHOLOGY_KERNEL, KERNEL_ANCHOR, dilate_size);
        Core.absdiff(sureBackgroundMaskMat, new Scalar(1), sureBackgroundMaskMat); // 1 - background

        Core.multiply(sureForegroundMaskMat, FOREGROUND, sureForegroundMaskMat);
        Core.multiply(sureBackgroundMaskMat, BACKGROUND, sureBackgroundMaskMat);

        Core.add(sureForegroundMaskMat, sureBackgroundMaskMat, watershedMaskMat);

        watershedMaskMat.convertTo(watershedMaskMat, CvType.CV_32S);
    }

    private Mat WatershedSegmentation(Mat inImageMat, Mat inMaskMat) {
        CreateWatershedMask(inMaskMat);
        if (_outputResultType == OutputResultTypeEnum.Final) {

            Imgproc.watershed(inImageMat, watershedMaskMat);
            Core.compare(watershedMaskMat, FOREGROUND, watershedMaskMat, Core.CMP_EQ);
            Imgproc.dilate(watershedMaskMat, watershedMaskMat, MORPHOLOGY_KERNEL, KERNEL_ANCHOR, 1);
            Core.absdiff(watershedMaskMat, new Scalar(255), watershedMaskMat);
        }
        watershedMaskMat.convertTo(watershedMaskMat, CvType.CV_8UC1);
        return watershedMaskMat;
    }

    public void SetOutputResultType(OutputResultTypeEnum inOutputResultType) {
        _outputResultType = inOutputResultType;
    }
}
