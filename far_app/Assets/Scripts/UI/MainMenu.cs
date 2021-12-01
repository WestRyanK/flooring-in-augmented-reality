using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Text _totalDurationText;
    [SerializeField]
    private Text _segmentationDurationText;
    [SerializeField]
    private Text _extractLightingDurationText;

    public void UpdateImageProcessDuration(float inTotalDuration, float inSegmentationDuration, float inExtractLightingDuration)
    {
        if (_totalDurationText)
        {
            _totalDurationText.text = $"Total: {inTotalDuration} ms";
        }
        if (_segmentationDurationText)
        {
            _segmentationDurationText.text = $"Segmentation: {inSegmentationDuration} ms";
        }
        if (_extractLightingDurationText)
        {
            _extractLightingDurationText.text = $"Lighting: {inExtractLightingDuration} ms";
        }
    }

    public void ShowMainMenu()
    {
        gameObject.SetActive(true);
    }

    public void HideMainMenu()
    {
        gameObject.SetActive(false);
    }
}
