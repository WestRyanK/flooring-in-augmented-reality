using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PluginWrapper
{
    private const string FAR_INTERFACE_CLASS = "com.westryank.farplugin.FarInterface";

    public static void Init(SegmentationModelTypeEnum inSegmentationModelType, OutputResultTypeEnum inOutputResultType)
    {
        PluginUtils.StaticCall("Init", FAR_INTERFACE_CLASS, PluginUtils.GetAndroidContext(), (int)inSegmentationModelType, (int)inOutputResultType);
    }

    public static void SetInputImage(int[] inInputImage, int inWidth, int inHeight)
    {
        PluginUtils.StaticCall("SetInputImage", FAR_INTERFACE_CLASS, inInputImage, inWidth, inHeight);
    }

    public static byte[] SegmentInputImage()
    {
        return PluginUtils.StaticCall<byte[]>("SegmentInputImage", FAR_INTERFACE_CLASS);
    }

    public static byte[] ExtractInputImageShadows()
    {
        return PluginUtils.StaticCall<byte[]>("ExtractInputImageShadows", FAR_INTERFACE_CLASS);
    }

    public static void SetOutputResultType(OutputResultTypeEnum inOutputResultType)
    {
        PluginUtils.StaticCall("SetOutputResultType", FAR_INTERFACE_CLASS, (int)inOutputResultType);
    }

    public static void SetSegmentationModelType(SegmentationModelTypeEnum inSegmentationModelType)
    {
        PluginUtils.StaticCall("SetSegmentationModelType", FAR_INTERFACE_CLASS, (int)inSegmentationModelType);
    }
}
