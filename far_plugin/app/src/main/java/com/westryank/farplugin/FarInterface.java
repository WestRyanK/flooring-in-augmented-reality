package com.westryank.farplugin;

import android.content.Context;
import android.util.Log;

public class FarInterface {
    private static final String TAG = "FAR:Interface";
    private static FarEngine _farEngine;

    public static void Init(Context inAppContext, int inSegmentationModelType, int inOutputResultType) {
        OutputResultTypeEnum outputResultType = OutputResultTypeEnum.FromInt(inOutputResultType);
        _farEngine = new FarEngine(inAppContext, SegmentationModelTypeEnum.FromInt(inSegmentationModelType), outputResultType);
    }

    public static void SetInputImage(int[] inInputImage, int inWidth, int inHeight) {
        if (AssertFarEngine("SetInputImage"))
            return;
        _farEngine.SetInputImage(inInputImage, inWidth, inHeight);
    }

    public static byte[] SegmentInputImage() {
        if (AssertFarEngine("SegmentInputImage"))
            return new byte[0];
        return _farEngine.SegmentInputImage();
    }

    public static byte[] ExtractInputImageShadows() {
        if (AssertFarEngine("ExtractInputImageShadows"))
            return new byte[0];
        return _farEngine.ExtractInputImageShadows();
    }

    public static void SetOutputResultType(int inOutputResultType) {
        if (AssertFarEngine("SetOutputResultType"))
            return;
        OutputResultTypeEnum value = OutputResultTypeEnum.FromInt(inOutputResultType);
        Log.v(TAG, "SetOutputResultType: " + Integer.toString(inOutputResultType) + " " + value.toString());
        _farEngine.InitSegmentationTuner(value);
    }

    public static void SetSegmentationModelType(int inSegmentationModelType) {
        if (AssertFarEngine("SetSegmentationModelType"))
            return;
        SegmentationModelTypeEnum value = SegmentationModelTypeEnum.FromInt(inSegmentationModelType);
        Log.v(TAG, "SetSegmentationModelType: " + Integer.toString(inSegmentationModelType) + " " + value.toString());
        _farEngine.InitSegmentationModel(value);
    }

    private static boolean AssertFarEngine(String inName) {
        boolean isNull = (_farEngine == null);
        if (isNull) {
            Log.e(TAG, "FarEngine was null in " + inName + ". Have you called Init yet?");
        }
        return isNull;
    }
}
