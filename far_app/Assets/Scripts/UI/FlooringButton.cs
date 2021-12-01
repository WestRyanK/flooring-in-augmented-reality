using UnityEngine;
using UnityEngine.UI;
using System;

public class FlooringButton : MonoBehaviour
{
    public event Action<FlooringOptionEnum> OnFlooringButtonClicked;
    public FlooringOptionEnum _flooringOption;
    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    void OnDestroy()
    {
        _button.onClick.RemoveListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        OnFlooringButtonClicked?.Invoke(_flooringOption);
    }
}