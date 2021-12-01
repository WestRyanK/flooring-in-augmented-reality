package com.westryank.farplugin;

import android.graphics.Bitmap;

import org.opencv.android.Utils;
import org.opencv.core.Core;
import org.opencv.core.CvType;
import org.opencv.core.Mat;
import org.opencv.core.Scalar;
import org.opencv.core.Size;
import org.opencv.imgproc.Imgproc;

public class LightingExtractor implements AutoCloseable{
    private static final String TAG = "Far:LightingExtractor";
    private static final int GRAY = 128;
    private static final int DARKEN_BIAS = 38;

    private Mat _imageMat;
    private Mat _maskMat;
    private Mat _blurredMaskMat;
    Bitmap _outputBitmap;

    public LightingExtractor() {
        _imageMat = new Mat();
        _maskMat = new Mat();
        _blurredMaskMat = new Mat();
    }

    private void Clamp(Mat inMat) {
        Imgproc.threshold(inMat, inMat, 0, 255, Imgproc.THRESH_TOZERO);
        Imgproc.threshold(inMat, inMat, 255, 255, Imgproc.THRESH_TRUNC);
    }

    public Bitmap ExtractLighting(Bitmap inImage, Bitmap inMask) {
        Utils.bitmapToMat(inImage, _imageMat);
        Utils.bitmapToMat(inMask, _maskMat);
        Core.extractChannel(_maskMat, _maskMat, 0);
        Core.rotate(_maskMat, _maskMat, Core.ROTATE_180);
        Core.absdiff(_maskMat, new Scalar(255), _maskMat);
        _maskMat.convertTo(_blurredMaskMat, CvType.CV_32FC1, 1.0f / 255);

        Size blurKernel = new Size(11, 11);
        Imgproc.cvtColor(_imageMat, _imageMat, Imgproc.COLOR_BGRA2GRAY);
        Core.multiply(_imageMat, _maskMat, _imageMat, 1.0f / 255);

        Imgproc.blur(_imageMat, _imageMat, blurKernel);
        Imgproc.blur(_blurredMaskMat, _blurredMaskMat, blurKernel);

        _imageMat.convertTo(_imageMat, CvType.CV_32FC1);
        Core.divide(_imageMat, _blurredMaskMat, _imageMat);
        _imageMat.convertTo(_imageMat, CvType.CV_8UC1);

        Scalar avgBrightness = Core.mean(_imageMat, _maskMat);
//        Scalar medBrightness = new Scalar(median(imageMat, maskMat));
        Scalar avgBrightnessOffset = new Scalar((GRAY - DARKEN_BIAS) - avgBrightness.val[0]);
        Core.add(_imageMat, avgBrightnessOffset, _imageMat);
        Clamp(_imageMat);

        Core.rotate(_imageMat, _imageMat, Core.ROTATE_180);
        Imgproc.cvtColor(_imageMat, _imageMat, Imgproc.COLOR_GRAY2BGRA);

        _outputBitmap = OpenCVUtils.CreateOutputBitmap(_outputBitmap, _imageMat);

        return _outputBitmap;
    }

    @Override
    public void close() throws Exception {
        _imageMat.release();
        _maskMat.release();
        _blurredMaskMat.release();
        _outputBitmap.recycle();
    }

//    double median(Mat inMat, Mat inMask) {
//        int binCount = 256;
//        int maxValue = 255;
//        List<Mat> imgs = new ArrayList<>();
//        imgs.add(inMat);
//        MatOfInt channels = new MatOfInt(0);
//        Mat histogram = new Mat();
//        MatOfInt histogramSize = new MatOfInt(binCount);
//        MatOfFloat histogramRanges = new MatOfFloat(0, maxValue);
//        Imgproc.calcHist(imgs, channels, inMask, histogram, histogramSize, histogramRanges, false);
//
//        float[] valueArray = new float[1];
//        double histogramTotal = Core.sumElems(histogram).val[0];
//        long halfCount = (long)(histogramTotal / 2);
//        long sumTotal = 0;
//        int index = 0;
//        for (int i = 0; i < binCount; i++) {
//            histogram.get(i, 0, valueArray);
//            sumTotal += valueArray[0];
//            if (sumTotal > halfCount) {
//                index = i;
//                break;
//            }
//        }
//        float binPercent = ((float)(index) / binCount);
//        int medianValue = (int)(binPercent * maxValue);
//        return medianValue;
//    }
}
