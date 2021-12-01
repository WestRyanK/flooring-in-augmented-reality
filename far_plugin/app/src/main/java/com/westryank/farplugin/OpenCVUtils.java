package com.westryank.farplugin;

import android.graphics.Bitmap;
import android.util.Log;

import org.opencv.android.OpenCVLoader;
import org.opencv.android.Utils;
import org.opencv.core.Core;
import org.opencv.core.CvType;
import org.opencv.core.Mat;

import java.util.ArrayList;
import java.util.List;

public class OpenCVUtils {
    private static final String TAG = "Far:OpenCVInitializer";
    private static boolean _isOpenCVInitialized = false;

    public static void InitializeOpenCV() {
        Log.v(TAG, "Initializing OpenCV");
        if (_isOpenCVInitialized)
        {
            Log.v(TAG, "OpenCV already initialized");
            return;
        }

        if (!OpenCVLoader.initDebug()) {
            Log.e(TAG, "Error loading OpenCV");
        } else {
            Log.v(TAG, "OpenCV loaded successfully");
        }
    }

    public static String GetMatDetails(String inMatName, Mat inMat) {

        if (inMat == null) {
            return inMatName + " mat is null!";
        }
        if (inMat.empty()) {
            return inMatName + " is empty!";
        }
        StringBuilder sb = new StringBuilder();
        sb.append(inMatName);
        sb.append(" ");
        sb.append(inMat.rows());
        sb.append("x");
        sb.append(inMat.cols());
        sb.append(" ");
        sb.append(CvType.typeToString(inMat.type()));

        List<Mat> channels = new ArrayList<>();
        Core.split(inMat, channels);
        for (int i = 0; i < inMat.channels(); i++) {
            sb.append(" ch ");
            sb.append(i);

            Core.MinMaxLocResult minMax = Core.minMaxLoc(channels.get(i));
            sb.append(" m ");
            sb.append(minMax.minVal);
            sb.append(" M ");
            sb.append(minMax.maxVal);
        }
        return sb.toString();
    }

    public static Bitmap CreateOutputBitmap(Bitmap inBitmap, Mat inMat) {
        if (inBitmap == null || inBitmap.getWidth() != inMat.width() || inBitmap.getHeight() != inMat.height()) {
            if (inBitmap != null) {
                inBitmap.recycle();
            }
            inBitmap = Bitmap.createBitmap(inMat.width(), inMat.height(), Bitmap.Config.ARGB_8888);
        }
        Utils.matToBitmap(inMat, inBitmap);
        return inBitmap;
    }
}
