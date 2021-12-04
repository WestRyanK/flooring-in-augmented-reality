using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlooringMenu : BasePopup
{
    [SerializeField]
    private FlooringButton _flooringButtonPrefab;
    [SerializeField]
    private GameObject _flooringOptionsParent;
    [SerializeField]
    private int _rotationAmount;
    private FarSettings _farSettings;

    void Start()
    {
        PopulateFlooringOptions();
    }

    void Awake()
    {
        _farSettings = FindObjectOfType<FarSettings>();
    }


    private void PopulateFlooringOptions()
    {
        if (_flooringButtonPrefab && _flooringOptionsParent && _farSettings)
        {
            foreach (FlooringOptionEnum flooringOption in _farSettings._flooringOptionTextures.Keys)
            {
                FlooringButton flooringButton = Instantiate<FlooringButton>(_flooringButtonPrefab, _flooringOptionsParent.transform);
                Image buttonImage = flooringButton.GetComponent<Image>();
                Texture2D flooringTexture = _farSettings._flooringOptionTextures[flooringOption];
                buttonImage.sprite = Sprite.Create(
                    flooringTexture,
                    new Rect(0, 0, flooringTexture.width, flooringTexture.height),
                    new Vector2(0.5f, 0.5f));
                flooringButton.gameObject.SetActive(true);
                flooringButton._flooringOption = flooringOption;
                flooringButton.OnFlooringButtonClicked += OnFlooringButtonClicked;
            }
        }
    }

    private void OnFlooringButtonClicked(FlooringOptionEnum inFlooringOption)
    {
        if (_farSettings)
        {
            _farSettings.FlooringOption = inFlooringOption;
        }
    }

    public void OnRotateFlooringButtonClicked(int inDirection)
    {
        if (_farSettings)
        {
            _farSettings.FlooringRotation += inDirection * _rotationAmount;
        }
    }
}
