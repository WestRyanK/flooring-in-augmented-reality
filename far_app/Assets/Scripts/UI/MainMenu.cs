using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private FloorSurfaceManager _floorSurfaceManager;
    [SerializeField]
    private Text _totalDurationText;
    [SerializeField]
    private Text _segmentationDurationText;
    [SerializeField]
    private Text _extractShadowDurationText;

    public void UpdateImageProcessDuration(float inTotalDuration, float inSegmentationDuration, float inExtractShadowDuration)
    {
        if (_totalDurationText)
        {
            _totalDurationText.text = $"Total: {inTotalDuration}";
        }
        if (_segmentationDurationText)
        {
            _segmentationDurationText.text = $"Segmentation: {inSegmentationDuration}";
        }
        if (_extractShadowDurationText)
        {
            _extractShadowDurationText.text = $"Shadow: {inExtractShadowDuration}";
        }
    }
}
