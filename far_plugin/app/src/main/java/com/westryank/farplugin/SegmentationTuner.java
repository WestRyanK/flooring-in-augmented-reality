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

    public SegmentationTuner(OutputResultTypeEnum inOutputResultType) {
        OpenCVUtils.InitializeOpenCV();

        MORPHOLOGY_KERNEL = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(3, 3));
        KERNEL_ANCHOR = new Point(-1, -1);
        SetOutputResultType(inOutputResultType);
    }

    public void Dispose() {
        if (MORPHOLOGY_KERNEL != null) {
            MORPHOLOGY_KERNEL.release();
        }
    }

    public Bitmap ConvertMaskBitmap(Bitmap inMask) {
        Mat mask = BitmapToMaskMat(inMask);
        Core.absdiff(mask, new Scalar(1), mask);
        Imgproc.cvtColor(mask, mask, Imgproc.COLOR_GRAY2BGRA);
        Core.multiply(mask, new Scalar(255, 255, 255, 1), mask);
        Core.rotate(mask, mask, Core.ROTATE_180);
        Bitmap outputBitmap = Bitmap.createBitmap(inMask.getWidth(), inMask.getHeight(), Bitmap.Config.ARGB_8888);
        Utils.matToBitmap(mask, outputBitmap);
        mask.release();
        return outputBitmap;
    }

    private Mat BitmapToMaskMat(Bitmap inMask) {
        Mat mask = new Mat();
        Utils.bitmapToMat(inMask, mask);
        Core.extractChannel(mask, mask, 3);
        Imgproc.threshold(mask, mask, 0, 1, Imgproc.THRESH_BINARY);
        return mask;
    }

    private Mat BitmapToImageMat(Bitmap inImage) {
        Mat image = new Mat();
        Utils.bitmapToMat(inImage, image);
        Imgproc.cvtColor(image, image, Imgproc.COLOR_BGRA2BGR);
        return image;
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
        Mat mask = BitmapToMaskMat(inMask);
        Mat image = BitmapToImageMat(inInputBitmap);
        Size originalSize = mask.size();
        if (_intermediateSize != 1.0f) {
            Imgproc.resize(mask, mask, new Size(), _intermediateSize, _intermediateSize, Imgproc.INTER_LINEAR);
            Imgproc.resize(image, image, new Size(), _intermediateSize, _intermediateSize, Imgproc.INTER_LINEAR);
        }

        Mat segmentation = WatershedSegmentation(image, mask);
        Imgproc.cvtColor(segmentation, segmentation, Imgproc.COLOR_GRAY2BGRA);
        if (_outputResultType == OutputResultTypeEnum.Final) {
            Core.multiply(segmentation, new Scalar(255, 255, 255, 1), segmentation);
        } else if (_outputResultType == OutputResultTypeEnum.SegmentationMask) {
            Core.multiply(segmentation, new Scalar(127, 127, 127, 1), segmentation);
        }

        if (_intermediateSize != 1.0f) {
            Imgproc.resize(segmentation, segmentation, originalSize, 0, 0, Imgproc.INTER_NEAREST);
        }
        Core.rotate(segmentation, segmentation, Core.ROTATE_180);
//        Imgproc.rectangle(segmentation, new Point(0, 0), new Point(segmentation.width(), segmentation.height()), new Scalar(0, 0, 0, 1), 10);
        // TODO: Optimize so we don't have to create this bitmap each time.
        Bitmap outputBitmap = Bitmap.createBitmap(inInputBitmap.getWidth(), inInputBitmap.getHeight(), Bitmap.Config.ARGB_8888);
        Utils.matToBitmap(segmentation, outputBitmap);

        Log.d(TAG, "OutputBmp: " + Integer.toString(outputBitmap.getWidth()) + " " + Integer.toString(outputBitmap.getHeight()) + " " + outputBitmap.getConfig().toString());
        mask.release();
        image.release();
        segmentation.release();

        return outputBitmap;
    }

    private Mat CreateWatershedMask(Mat inApproxForegroundMask) {
        Mat sureForegroundMask = new Mat();
        int erode_size = (int) (21 * _intermediateSize); // 21
        Imgproc.erode(inApproxForegroundMask, sureForegroundMask, MORPHOLOGY_KERNEL, KERNEL_ANCHOR, erode_size);

        Mat sureBackgroundMask = new Mat();
        int dilate_size = (int) (21 * _intermediateSize); // 41
        Imgproc.dilate(inApproxForegroundMask, sureBackgroundMask, MORPHOLOGY_KERNEL, KERNEL_ANCHOR, dilate_size);
        Core.absdiff(sureBackgroundMask, new Scalar(1), sureBackgroundMask); // 1 - background

        Core.multiply(sureForegroundMask, FOREGROUND, sureForegroundMask);
        Core.multiply(sureBackgroundMask, BACKGROUND, sureBackgroundMask);

        Mat watershedMask = new Mat();
        Core.add(sureForegroundMask, sureBackgroundMask, watershedMask);

        sureForegroundMask.release();
        sureBackgroundMask.release();

        watershedMask.convertTo(watershedMask, CvType.CV_32S);

        return watershedMask;
    }

    private Mat WatershedSegmentation(Mat inImage, Mat inMask) {
        Mat watershedMask = CreateWatershedMask(inMask);
        if (_outputResultType == OutputResultTypeEnum.Final) {

            Imgproc.watershed(inImage, watershedMask);
            Core.compare(watershedMask, BACKGROUND, watershedMask, Core.CMP_EQ);
        }
        watershedMask.convertTo(watershedMask, CvType.CV_8UC1);
        return watershedMask;
    }

    public void SetOutputResultType(OutputResultTypeEnum inOutputResultType) {
        _outputResultType = inOutputResultType;
    }
}
