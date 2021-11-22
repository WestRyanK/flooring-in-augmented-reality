using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceProjector : MonoBehaviour
{
    private Projector _projector;
    private Camera _camera;

    private Vector3 _savedCameraPosition;
    private Quaternion _savedCameraOrientation;
    private float _savedCameraFov;

    void Start()
    {
        _camera = FindObjectOfType<Camera>();
        // Debug.Log($"Projector: {gameObject} Camera: {_camera}");
        _projector = GetComponent<Projector>();
    }

    public void SetProjectionTexture(Texture2D inTexture)
    {
        if (_projector != null)
        {
        // Debug.Log("ProjectionTexture Set###");
            _projector.material.mainTexture = inTexture;
            float textureAspectRatio = (float)inTexture.width / inTexture.height;
            _projector.aspectRatio = textureAspectRatio;
        }
    }

    public void SaveCameraTransform()
    {
        // Debug.Log($"Saving Camera position. Projector: {gameObject} Camera: {_camera}");
        if (_camera == null)
        {
            Debug.Log("Camera for SurfaceProjector is null");
            return;
        }
        _savedCameraPosition = _camera.transform.position;
        _savedCameraOrientation = _camera.transform.rotation;
        _savedCameraFov = _camera.fieldOfView;
    }

    public void MoveToSavedCameraTransform()
    {

        if (_camera != null)
        {
            // Debug.Log($"Moving Projector {gameObject} to Saved Camera position");
            transform.position = _savedCameraPosition;
            transform.rotation = _savedCameraOrientation;
            _projector.fieldOfView = _savedCameraFov;
        }
    }
}
