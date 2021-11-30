package com.westryank.farplugin;

import android.graphics.Bitmap;
import android.util.Log;

import org.opencv.android.Utils;
import org.opencv.core.Core;
import org.opencv.core.CvType;
import org.opencv.core.Mat;
import org.opencv.core.MatOfFloat;
import org.opencv.core.MatOfInt;
import org.opencv.core.Scalar;
import org.opencv.core.Size;
import org.opencv.imgproc.Imgproc;

import java.util.ArrayList;
import java.util.List;

public class LightingExtractor {
    private static final String TAG = "Far:LightingExtractor";
    private static final int GRAY = 128;
    private static final int DARKEN_BIAS = 38;

    private void Clamp(Mat inMat) {
        Imgproc.threshold(inMat, inMat, 0, 255, Imgproc.THRESH_TOZERO);
        Imgproc.threshold(inMat, inMat, 255, 255, Imgproc.THRESH_TRUNC);
    }

    public Bitmap ExtractLighting(Bitmap inImage, Bitmap inMask) {
        Mat imageMat = new Mat();
        Utils.bitmapToMat(inImage, imageMat);
        Mat maskMat = new Mat();
        Utils.bitmapToMat(inMask, maskMat);
        Core.extractChannel(maskMat, maskMat, 0);
        Core.rotate(maskMat, maskMat, Core.ROTATE_180);
        Core.absdiff(maskMat, new Scalar(255), maskMat);
        Mat blurredMaskMat = new Mat();
        maskMat.convertTo(blurredMaskMat, CvType.CV_32FC1, 1.0f / 255);

        Size blurKernel = new Size(11, 11);
        Imgproc.cvtColor(imageMat, imageMat, Imgproc.COLOR_BGRA2GRAY);
        Core.multiply(imageMat, maskMat, imageMat, 1.0f / 255);

        Imgproc.blur(imageMat, imageMat, blurKernel);
        Imgproc.blur(blurredMaskMat, blurredMaskMat, blurKernel);

        imageMat.convertTo(imageMat, CvType.CV_32FC1);
        Core.divide(imageMat, blurredMaskMat, imageMat);
        imageMat.convertTo(imageMat, CvType.CV_8UC1);

        Scalar avgBrightness = Core.mean(imageMat, maskMat);
//        Scalar medBrightness = new Scalar(median(imageMat, maskMat));
        Scalar avgBrightnessOffset = new Scalar((GRAY - DARKEN_BIAS) - avgBrightness.val[0]);
        Core.add(imageMat, avgBrightnessOffset, imageMat);
        Clamp(imageMat);

        Core.rotate(imageMat, imageMat, Core.ROTATE_180);
        Imgproc.cvtColor(imageMat, imageMat, Imgproc.COLOR_GRAY2BGRA);

        Bitmap outputBitmap = Bitmap.createBitmap(inImage.getWidth(), inImage.getHeight(), Bitmap.Config.ARGB_8888);
        Utils.matToBitmap(imageMat, outputBitmap);

        imageMat.release();
        maskMat.release();
        blurredMaskMat.release();

        return outputBitmap;
    }

    double median(Mat inMat, Mat inMask) {
        int binCount = 256;
        int maxValue = 255;
        List<Mat> imgs = new ArrayList<>();
        imgs.add(inMat);
        MatOfInt channels = new MatOfInt(0);
        Mat histogram = new Mat();
        MatOfInt histogramSize = new MatOfInt(binCount);
        MatOfFloat histogramRanges = new MatOfFloat(0, maxValue);
        Imgproc.calcHist(imgs, channels, inMask, histogram, histogramSize, histogramRanges, false);

        float[] valueArray = new float[1];
        double histogramTotal = Core.sumElems(histogram).val[0];
        long halfCount = (long)(histogramTotal / 2);
        long sumTotal = 0;
        int index = 0;
        for (int i = 0; i < binCount; i++) {
            histogram.get(i, 0, valueArray);
            sumTotal += valueArray[0];
            if (sumTotal > halfCount) {
                index = i;
                break;
            }
        }
        float binPercent = ((float)(index) / binCount);
        int medianValue = (int)(binPercent * maxValue);
        return medianValue;
    }
}
