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
    private SegmentationModelTypeEnum _segmentationModelType = SegmentationModelTypeEnum.model_257_osade_weighted;
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
            if (_lightingProjector)
            {
                bool isLightingOn = _lightingType == LightingTypeEnum.lightingOn;
                _lightingProjector.SetActive(isLightingOn);
            }
        }
    }

    [SerializeField]
    private FlooringOptionEnum _flooringOption = FlooringOptionEnum.tile1;
    public FlooringOptionEnum FlooringOption
    {
        get
        {
            return _flooringOption;
        }
        set
        {
            _flooringOption = value;
            if (_lavaPlane && _flooringPlane)
            {
                bool isLavaOn = _flooringOption == FlooringOptionEnum.lava;
                _lavaPlane.SetActive(isLavaOn);
                _flooringPlane.SetActive(!isLavaOn);
                if (!isLavaOn)
                {
                    Texture2D flooringTexture = _flooringOptionTextures[_flooringOption];
                    _flooringPlane.GetComponent<Renderer>().material.mainTexture = flooringTexture;
                }
            }

        }
    }

    [SerializeField]
    private List<FlooringOptionEnumToTexture2D> _flooringOptionTexturesList;
    public Dictionary<FlooringOptionEnum, Texture2D> _flooringOptionTextures;
    [SerializeField]
    private GameObject _flooringPlane;
    [SerializeField]
    private GameObject _lavaPlane;
    [SerializeField]
    private GameObject _debugImageViewer;
    [SerializeField]
    private GameObject _lightingProjector;

    void Awake()
    {
        BuildFlooringOptionEnumToTexture();
    }

    private void BuildFlooringOptionEnumToTexture()
    {
        _flooringOptionTextures = new Dictionary<FlooringOptionEnum, Texture2D>();
        foreach (FlooringOptionEnumToTexture2D pair in _flooringOptionTexturesList)
        {
            _flooringOptionTextures.Add(pair._flooringOptionEnum, pair._texture2D);
        }
    }

}
