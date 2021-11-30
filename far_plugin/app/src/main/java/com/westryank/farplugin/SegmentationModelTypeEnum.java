package com.westryank.farplugin;

public enum SegmentationModelTypeEnum {
    model_513("deeplab_mnv2_513.tflite"),
    model_257("deeplab_mnv2_257_osade.tflite"),
    model_129("deeplab_mnv2_129_osade.tflite");

    private final String _filename;

    SegmentationModelTypeEnum(final String inFilename) {
        _filename = inFilename;
    }

    public static SegmentationModelTypeEnum FromInt(int inValue) {
        return SegmentationModelTypeEnum.values()[inValue];
    }


    public String GetFilename() {
        return _filename;
    }
}
