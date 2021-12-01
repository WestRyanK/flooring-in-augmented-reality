using System.Collections;
using System.Collections.Generic;

// If you change these values, also update them in far_plugin project.
public enum SegmentationModelTypeEnum
{
    [EnumName("513")]
    model_513 = 0,
    [EnumName("257 OSADE Weight")]
    model_257_osade_weighted = 1,
    [EnumName("129")]
    model_129 = 2,
    [EnumName("513 OSADE")]
    model_513_osade = 3,
    [EnumName("513 OSADE Weight")]
    model_513_osade_weighted = 4
}