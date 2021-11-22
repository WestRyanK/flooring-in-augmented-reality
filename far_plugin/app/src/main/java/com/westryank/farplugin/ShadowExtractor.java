package com.westryank.farplugin;

import android.graphics.Bitmap;

import org.opencv.android.Utils;
import org.opencv.core.Core;
import org.opencv.core.Mat;
import org.opencv.core.Scalar;
import org.opencv.core.Size;
import org.opencv.imgproc.Imgproc;

public class ShadowExtractor {
    private static final String TAG = "Far:ShadowExtractor";

    public Bitmap ExtractShadows(Bitmap inImage, Bitmap inMask) {
        Mat imageMat = new Mat();
        Utils.bitmapToMat(inImage, imageMat);
        Mat maskMat = new Mat();
        Utils.bitmapToMat(inMask, maskMat);

        Imgproc.cvtColor(imageMat, imageMat, Imgproc.COLOR_BGRA2GRAY);
        Imgproc.blur(imageMat, imageMat, new Size(11, 11));
        Scalar avgBrightness = Core.mean(imageMat, maskMat);
        Scalar avgBrightnessOffset = new Scalar(128 - avgBrightness.val[0]);
        Core.add(imageMat, avgBrightnessOffset, imageMat);
        Imgproc.cvtColor(imageMat, imageMat, Imgproc.COLOR_GRAY2BGRA);

        Bitmap outputBitmap = Bitmap.createBitmap(inImage.getWidth(), inImage.getHeight(), Bitmap.Config.ARGB_8888);
        Utils.matToBitmap(imageMat, outputBitmap);

        imageMat.release();
        maskMat.release();

        return outputBitmap;
    }
}
