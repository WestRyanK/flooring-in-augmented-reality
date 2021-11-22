using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FarSettings : MonoBehaviour
{
    [SerializeField]
    private OutputResultTypeEnum _outputResultType = OutputResultTypeEnum.final;
    public OutputResultTypeEnum OutputResultType
    {
        get
        {
            return _outputResultType;
        }
        set
        {
            _outputResultType = value;
            PluginWrapper.SetOutputResultType(_outputResultType);

            bool isDebugImageVisible = _outputResultType == OutputResultTypeEnum.segmentationMask;
            if (_debugImageViewer)
            {
                _debugImageViewer.SetActive(isDebugImageVisible);
            }
        }
    }

    [SerializeField]
    private SegmentationModelTypeEnum _segmentationModelType = SegmentationModelTypeEnum.model_257;
    public SegmentationModelTypeEnum SegmentationModelType
    {
        get
        {
            return _segmentationModelType;
        }
        set
        {
            _segmentationModelType = value;
            PluginWrapper.SetSegmentationModelType(_segmentationModelType);
        }
    }

    [SerializeField]
    private LightingTypeEnum _lightingType = LightingTypeEnum.lightingOn;
    public LightingTypeEnum LightingType
    {
        get
        {
            return _lightingType;
        }
        set
        {
            _lightingType = value;
            // PluginWrapper.SetLightingType(_lightingType.Value);
        }
    }

    [SerializeField]
    private GameObject _debugImageViewer;
}
