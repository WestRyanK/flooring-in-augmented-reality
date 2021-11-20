package com.westryank.farplugin;

public enum OutputResultTypeEnum {
    Final(0),
    SegmentationMask(1),
    NoFineTuning(2);

    public final int value;

    private OutputResultTypeEnum(int inValue) {
        value = inValue;
    }

    public static OutputResultTypeEnum FromInt(int inValue) {
        return OutputResultTypeEnum.values()[inValue];
    }
}
