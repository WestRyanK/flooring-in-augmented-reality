using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Unity.Collections;
using System.Threading.Tasks;

public class FloorSurfaceManager : MonoBehaviour
{
    [SerializeField]
    private MovementThresholder _cameraMovementThresholder;
    [SerializeField]
    private UnityEvent _onProcessCameraImageStart;
    [SerializeField]
    private UnityEvent<float, float, float> _onProcessCameraImageComplete;
    [SerializeField]
    private UnityEvent<Texture2D> _onSegmentImageComplete;
    [SerializeField]
    private UnityEvent<Texture2D> _onExtractLightingComplete;
    private Texture2D _segmentationTexture;
    private Texture2D _lightingTexture;
    private CameraImageFetcher _cameraImageFetcher;
    private bool _isReadyForNextImage;
    private NativeList<byte> _cameraImage;
    private FarSettings _settings;

    void Start()
    {
        _settings = FindObjectOfType<FarSettings>();
        _cameraImageFetcher = GetComponent<CameraImageFetcher>();
        PluginWrapper.Init(_settings.SegmentationModelType, _settings.OutputResultType);

        _cameraImage = new NativeList<byte>(Allocator.Persistent);
        InitProjectorTexture(480, 640, ref _segmentationTexture);
        InitProjectorTexture(480, 640, ref _lightingTexture);
        _isReadyForNextImage = true;
    }

    void Update()
    {
        if (_isReadyForNextImage && _cameraMovementThresholder.HasExceededMovementThreshold())
        {
            Debug.Log("Segmenting Image");
            StartCoroutine(ProcessCameraImage());
        }
    }

    void OnDestroy()
    {
        _cameraImage.Dispose();
    }

    private IEnumerator ProcessCameraImage()
    {
        _isReadyForNextImage = false;
        if (_cameraImageFetcher.GetCameraImage(ref _cameraImage, out int width, out int height))
        {
            _onProcessCameraImageStart.Invoke();

            byte[] segmentationTextureData = null;
            byte[] lightingTextureData = null;
            Stopwatch totalStopwatch = null;
            Stopwatch segmentationStopwatch = null;
            Stopwatch extractLightingStopwatch = null;
            yield return WaitForTask(() =>
            {
                using (totalStopwatch = new Stopwatch())
                {
                    int[] cameraImage = CameraImageFetcher.ConvertCameraBytesToInts(_cameraImage, ref width, ref height);
                    PluginWrapper.SetInputImage(cameraImage, width, height);
                    using (segmentationStopwatch = new Stopwatch())
                    {
                        segmentationTextureData = PluginWrapper.SegmentInputImage();
                    }
                    using (extractLightingStopwatch = new Stopwatch())
                    {
                        lightingTextureData = PluginWrapper.ExtractInputImageLighting();
                    }
                }
            });

            UpdateTexture(segmentationTextureData, ref _segmentationTexture);
            UpdateTexture(lightingTextureData, ref _lightingTexture);

            Debug.Log($"SegmentationTexture: {_segmentationTexture}");
            _onSegmentImageComplete.Invoke(_segmentationTexture);
            Debug.Log($"LightingTexture: {_lightingTexture}");
            _onExtractLightingComplete.Invoke(_lightingTexture);
            _onProcessCameraImageComplete.Invoke(
                totalStopwatch?.Duration ?? 0,
                segmentationStopwatch?.Duration ?? 0,
                extractLightingStopwatch?.Duration ?? 0);
        }
        else
        {
            Debug.Log("Failed to get camera image. Not segmenting");
        }
        _isReadyForNextImage = true;
        yield break;
    }

    private IEnumerator WaitForTask(System.Action inTask)
    {
        bool isTaskComplete = false;
        Task.Run(() =>
        {
            inTask.Invoke();
            isTaskComplete = true;
        });
        while (!isTaskComplete)
        {
            yield return null;
        }
    }

    private void InitProjectorTexture(int inWidth, int inHeight, ref Texture2D refTexture)
    {
        refTexture = new Texture2D(inWidth, inHeight, TextureFormat.RGBA32, false);
        refTexture.filterMode = FilterMode.Point;
        refTexture.wrapMode = TextureWrapMode.Clamp;
    }

    private void UpdateTexture(byte[] inTextureData, ref Texture2D refTexture)
    {
        refTexture.LoadRawTextureData(inTextureData);
        refTexture.Apply();
    }
}
