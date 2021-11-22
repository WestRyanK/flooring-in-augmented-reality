using System.Collections;
using System.Collections.Generic;

// If you change these values, also update them in far_plugin project.
public enum OutputResultTypeEnum
{
    [EnumName("Final")]
    final = 0,
    [EnumName("Segmentation Mask")]
    segmentationMask = 1,
    [EnumName("No Fine Tuning")]
    noFineTuning = 2
}