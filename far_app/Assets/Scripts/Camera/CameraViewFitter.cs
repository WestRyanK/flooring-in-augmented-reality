using UnityEngine;
using UnityEngine.UI;

public class CameraViewFitter : MonoBehaviour
{
    [SerializeField]
    private Vector2 _fitSize;
    [SerializeField]
    private AspectRatioFitter.AspectMode _fitMode;

    private Camera _camera;

    void Start()
    {
        _camera = GetComponentInParent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        FitCamera();
    }

    private void FitCamera()
    {
        transform.localPosition = new Vector3(0, 0, 1);
        Vector3[] corners = new Vector3[4];
        _camera.CalculateFrustumCorners(new Rect(0, 0, 1, 1), 1, Camera.MonoOrStereoscopicEye.Mono, corners);
        Vector2 viewScale = new Vector2(
            corners[2].x - corners[0].x,
            corners[2].y - corners[0].y);
        float width, height;
        float aspectRatio = _fitSize.x / _fitSize.y;

        switch (_fitMode)
        {
            case AspectRatioFitter.AspectMode.WidthControlsHeight:
                width = viewScale.x;
                height = width / aspectRatio;
                break;
            case AspectRatioFitter.AspectMode.HeightControlsWidth:
                height = viewScale.y;
                width = height * aspectRatio;
                break;
            case AspectRatioFitter.AspectMode.FitInParent:
                width = viewScale.x;
                height = viewScale.y;
                break;
            case AspectRatioFitter.AspectMode.EnvelopeParent:
            case AspectRatioFitter.AspectMode.None:
            default:
                width = 1;
                height = 1;
                break;
        }
        transform.localScale = new Vector3(width, height, 1);
    }
}
