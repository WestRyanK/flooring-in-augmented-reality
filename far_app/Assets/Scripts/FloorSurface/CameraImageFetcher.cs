using UnityEngine;
using Unity.Collections;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CameraImageFetcher : MonoBehaviour
{
    private ARCameraManager _cameraManager;
    private const int DEPTH = 3;

    void Start()
    {
        _cameraManager = FindObjectOfType<ARCameraManager>();
    }

    public bool GetCameraImage(ref NativeList<byte> refImageBytes, out int outWidth, out int outHeight)
    {
        outWidth = 0;
        outHeight = 0;

        if (!_cameraManager)
        {
            Debug.Log("CameraManager is null");
            return false;
        }

        Debug.Log($"Attempting to acquire image: {_cameraManager.currentConfiguration}|{_cameraManager.descriptor}|{_cameraManager.descriptor.supportsCameraImage}");
        if (!_cameraManager.TryAcquireLatestCpuImage(out XRCpuImage cameraImage))
        {
            Debug.Log("Can't acquire camera image");
            return false;
        }

        int imageSize = cameraImage.width * cameraImage.height * DEPTH;
        if (refImageBytes.Length != imageSize)
        {
            refImageBytes.Clear();
            refImageBytes.ResizeUninitialized(imageSize);
        }
        NativeSlice<byte> slice = new NativeSlice<byte>(refImageBytes);

        try
        {
            XRCpuImage.ConversionParams conversionParams = new XRCpuImage.ConversionParams(cameraImage, TextureFormat.RGB24);
            cameraImage.Convert(conversionParams, slice);
            outWidth = cameraImage.width;
            outHeight = cameraImage.height;
            return true;
        }
        catch (System.Exception) 
        {
            return false;
        }
        finally
        {
            if (cameraImage.valid)
            {
                cameraImage.Dispose();
            }
        }
    }

    public static int[] ConvertCameraBytesToInts(NativeList<byte> inCameraDataBytes, ref int refWidth, ref int refHeight)
    {
        int inSrcWidth = refWidth;
        int inSrcHeight = refHeight;
        int inDstWidth = refHeight;

        int[] cameraIntData = new int[inCameraDataBytes.Length];
        // Transpose, mirror x, mirror Y
        for (int y = 0; y < inSrcHeight; y++)
        {
            int srcY = inSrcWidth * y * DEPTH;
            // int srcY = srcWidth * (srcHeight - y - 1) * DEPTH;
            int dstX = y * DEPTH;
            for (int x = 0; x < inSrcWidth; x++)
            {
                int dstY = x * inDstWidth * DEPTH;
                int srcX = x * DEPTH;
                // int srcX = (srcWidth - x - 1) * DEPTH;
                cameraIntData[dstY + dstX + 0] = (int)inCameraDataBytes[srcY + srcX + 0];
                cameraIntData[dstY + dstX + 1] = (int)inCameraDataBytes[srcY + srcX + 1];
                cameraIntData[dstY + dstX + 2] = (int)inCameraDataBytes[srcY + srcX + 2];
            }
        }

        refWidth = inSrcHeight;
        refHeight = inSrcWidth;
        return cameraIntData;
    }
}
