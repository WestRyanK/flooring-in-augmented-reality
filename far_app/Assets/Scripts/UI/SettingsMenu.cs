using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SettingsMenu : BasePopup
{
    [SerializeField]
    private Dropdown _segmentationModelTypeDropdown;
    [SerializeField]
    private Dropdown _outputResultTypeDropdown;
    [SerializeField]
    private Dropdown _lightingTypeDropdown;

    [SerializeField]
    private Toggle _isStatsPanelVisibleToggle;
    [SerializeField]
    private Toggle _isPointCloudVisibleToggle;
    private FarSettings _farSettings;

    public void Start()
    {
        _farSettings = FindObjectOfType<FarSettings>();
        LoadDropdown(typeof(SegmentationModelTypeEnum), _segmentationModelTypeDropdown, (int)_farSettings.SegmentationModelType);
        LoadDropdown(typeof(OutputResultTypeEnum), _outputResultTypeDropdown, (int)_farSettings.OutputResultType);
        LoadDropdown(typeof(LightingTypeEnum), _lightingTypeDropdown, (int)_farSettings.LightingType);
        if (_isStatsPanelVisibleToggle)
        {
            _isStatsPanelVisibleToggle.isOn = _farSettings.IsStatsPanelVisible;
        }
        if (_isPointCloudVisibleToggle)
        {
            _isPointCloudVisibleToggle.isOn = _farSettings.IsPointCloudVisible;
        }
    }

    public void LoadDropdown(Type inEnumType, Dropdown inDropdown, int inInitialValue)
    {
        if (!inDropdown)
        {
            Debug.Log("Missing inDropdown");
            return;
        }

        inDropdown.options.Clear();
        List<string> options = inEnumType.GetAllNames();
        inDropdown.AddOptions(options);
        inDropdown.value = inInitialValue;
    }

    public void OnSegmentationModelTypeDropdownChanged(int inComboIndex)
    {
        SegmentationModelTypeEnum selectedValue = (SegmentationModelTypeEnum)inComboIndex;
        Debug.Log($"Set SegmentationModelType to {selectedValue}");
        _farSettings.SegmentationModelType = selectedValue;
    }

    public void OnOutputResultTypeDropdownChanged(int inComboIndex)
    {
        OutputResultTypeEnum selectedValue = (OutputResultTypeEnum)inComboIndex;
        Debug.Log($"Set OutputResultType to {selectedValue}");
        _farSettings.OutputResultType = selectedValue;
    }

    public void OnLightingTypeDropdownChanged(int inComboIndex)
    {
        LightingTypeEnum selectedValue = (LightingTypeEnum)inComboIndex;
        Debug.Log($"Set LightingType to {selectedValue}");
        _farSettings.LightingType = selectedValue;
    }

    public void OnStatsPanelVisibleChanged(bool inIsVisible)
    {
        _farSettings.IsStatsPanelVisible = inIsVisible;
    }

    public void OnPointCloudVisibleChanged(bool inIsVisible)
    {
        _farSettings.IsPointCloudVisible = inIsVisible;
    }
}
