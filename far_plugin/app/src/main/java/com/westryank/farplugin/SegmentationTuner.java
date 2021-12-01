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


public class SegmentationTuner implements AutoCloseable{
    private static final String TAG = "Far:SegmentationTuner";
    private static final Scalar FOREGROUND = new Scalar(1);
    private static final Scalar BACKGROUND = new Scalar(2);
    private static Mat MORPHOLOGY_KERNEL;
    private static Point KERNEL_ANCHOR;

    private OutputResultTypeEnum _outputResultType;
    private float _intermediateSize = 1.0f;

    private Mat _maskMat;
    private Mat _imageMat;
    private Mat _watershedMaskMat;
    private Mat _sureForegroundMaskMat;
    private Mat _sureBackgroundMaskMat;
    private Bitmap _outputBitmap;

    public SegmentationTuner(OutputResultTypeEnum inOutputResultType) {
        OpenCVUtils.InitializeOpenCV();

        MORPHOLOGY_KERNEL = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(3, 3));
        KERNEL_ANCHOR = new Point(-1, -1);
        SetOutputResultType(inOutputResultType);
        _maskMat = new Mat();
        _imageMat = new Mat();
        _watershedMaskMat = new Mat();
        _sureForegroundMaskMat = new Mat();
        _sureBackgroundMaskMat = new Mat();
    }

    public Bitmap ConvertMaskBitmap(Bitmap inMask) {
        BitmapToMaskMat(inMask);
        Core.absdiff(_maskMat, new Scalar(1), _maskMat);
        Imgproc.cvtColor(_maskMat, _maskMat, Imgproc.COLOR_GRAY2BGRA);
        Core.multiply(_maskMat, new Scalar(255, 255, 255, 1), _maskMat);
        Core.rotate(_maskMat, _maskMat, Core.ROTATE_180);
        _outputBitmap = OpenCVUtils.CreateOutputBitmap(_outputBitmap, _maskMat);
        return _outputBitmap;
    }

    private void BitmapToMaskMat(Bitmap inMask) {
        Utils.bitmapToMat(inMask, _maskMat);
        Core.extractChannel(_maskMat, _maskMat, 3);
        Imgproc.threshold(_maskMat, _maskMat, 0, 1, Imgproc.THRESH_BINARY);
    }

    private void BitmapToImageMat(Bitmap inImage) {
        Utils.bitmapToMat(inImage, _imageMat);
        Imgproc.cvtColor(_imageMat, _imageMat, Imgproc.COLOR_BGRA2BGR);
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

        BitmapToMaskMat(inMask);
        BitmapToImageMat(inInputBitmap);
        Size originalSize = _maskMat.size();
        if (_intermediateSize != 1.0f) {
            Imgproc.resize(_maskMat, _maskMat, new Size(), _intermediateSize, _intermediateSize, Imgproc.INTER_LINEAR);
            Imgproc.resize(_imageMat, _imageMat, new Size(), _intermediateSize, _intermediateSize, Imgproc.INTER_LINEAR);
        }

        Mat segmentationMat = WatershedSegmentation(_imageMat, _maskMat);
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
        _outputBitmap = OpenCVUtils.CreateOutputBitmap(_outputBitmap, segmentationMat);

        Log.d(TAG, "OutputBmp: " + Integer.toString(_outputBitmap.getWidth()) + " " + Integer.toString(_outputBitmap.getHeight()) + " " + _outputBitmap.getConfig().toString());

        return _outputBitmap;
    }

    private void CreateWatershedMask(Mat inApproxForegroundMaskMat) {
        int erode_size = (int) (21 * _intermediateSize); // 21
        Imgproc.erode(inApproxForegroundMaskMat, _sureForegroundMaskMat, MORPHOLOGY_KERNEL, KERNEL_ANCHOR, erode_size);

        int dilate_size = (int) (21 * _intermediateSize); // 41
        Imgproc.dilate(inApproxForegroundMaskMat, _sureBackgroundMaskMat, MORPHOLOGY_KERNEL, KERNEL_ANCHOR, dilate_size);
        Core.absdiff(_sureBackgroundMaskMat, new Scalar(1), _sureBackgroundMaskMat); // 1 - background

        Core.multiply(_sureForegroundMaskMat, FOREGROUND, _sureForegroundMaskMat);
        Core.multiply(_sureBackgroundMaskMat, BACKGROUND, _sureBackgroundMaskMat);

        Core.add(_sureForegroundMaskMat, _sureBackgroundMaskMat, _watershedMaskMat);

        _watershedMaskMat.convertTo(_watershedMaskMat, CvType.CV_32S);
    }

    private Mat WatershedSegmentation(Mat inImageMat, Mat inMaskMat) {
        CreateWatershedMask(inMaskMat);
        if (_outputResultType == OutputResultTypeEnum.Final) {

            Imgproc.watershed(inImageMat, _watershedMaskMat);
            Core.compare(_watershedMaskMat, FOREGROUND, _watershedMaskMat, Core.CMP_EQ);
            Imgproc.dilate(_watershedMaskMat, _watershedMaskMat, MORPHOLOGY_KERNEL, KERNEL_ANCHOR, 1);
            Core.absdiff(_watershedMaskMat, new Scalar(255), _watershedMaskMat);
        }
        _watershedMaskMat.convertTo(_watershedMaskMat, CvType.CV_8UC1);
        return _watershedMaskMat;
    }

    public void SetOutputResultType(OutputResultTypeEnum inOutputResultType) {
        _outputResultType = inOutputResultType;
    }

    @Override
    public void close() throws Exception {
        if (MORPHOLOGY_KERNEL != null) {
            MORPHOLOGY_KERNEL.release();
        }

        _maskMat.release();
        _imageMat.release();
        _watershedMaskMat.release();
        _sureForegroundMaskMat.release();
        _sureBackgroundMaskMat.release();
    }
}
