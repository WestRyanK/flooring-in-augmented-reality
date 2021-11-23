package com.westryank.farplugin;

import android.graphics.Bitmap;
import android.util.Log;

import org.opencv.android.Utils;
import org.opencv.core.Core;
import org.opencv.core.Mat;
import org.opencv.core.Scalar;
import org.opencv.core.Size;
import org.opencv.imgproc.Imgproc;

public class LightingExtractor {
    private static final String TAG = "Far:LightingExtractor";

    private void Clamp(Mat inMat)
    {
        Imgproc.threshold(inMat, inMat, 0, 255, Imgproc.THRESH_TOZERO);
        Imgproc.threshold(inMat, inMat, 255, 255, Imgproc.THRESH_TRUNC);
    }

    public Bitmap ExtractLighting(Bitmap inImage, Bitmap inMask) {
        Mat imageMat = new Mat();
        Utils.bitmapToMat(inImage, imageMat);
        Mat maskMat = new Mat();
        Utils.bitmapToMat(inMask, maskMat);
        Core.extractChannel(maskMat, maskMat, 0);

        Imgproc.cvtColor(imageMat, imageMat, Imgproc.COLOR_BGRA2GRAY);
        Imgproc.blur(imageMat, imageMat, new Size(11, 11));
        Log.v(TAG, OpenCVUtils.GetMatDetails("ImageMat", imageMat));
//        Scalar avgBrightness = Core.mean(imageMat, maskMat);
//        Scalar avgBrightnessOffset = new Scalar(128 - avgBrightness.val[0]);
//        Log.v(TAG, OpenCVUtils.GetMatDetails("Pre Offset ImageMat", imageMat));
//        Core.add(imageMat, avgBrightnessOffset, imageMat);
//        Log.v(TAG, OpenCVUtils.GetMatDetails("Post Offset ImageMat", imageMat));
//        Clamp(imageMat);
//        Log.v(TAG, OpenCVUtils.GetMatDetails("Post Clamp ImageMat", imageMat));

        Imgproc.cvtColor(imageMat, imageMat, Imgproc.COLOR_GRAY2BGRA);
        Core.rotate(imageMat, imageMat, Core.ROTATE_180);

        Bitmap outputBitmap = Bitmap.createBitmap(inImage.getWidth(), inImage.getHeight(), Bitmap.Config.ARGB_8888);
        Utils.matToBitmap(imageMat, outputBitmap);

        imageMat.release();
        maskMat.release();

        return outputBitmap;
    }
}
