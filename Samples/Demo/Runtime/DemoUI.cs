using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GGMakers.UIFramework;

public class DemoUI : UIController
{
    public System.Action onCloseCliced;

    [SerializeField]
    private Button _closeBtn;

    private void OnEnable()
    {
        _closeBtn?.onClick.AddListener(handleCloseButtonClicked);
    }

    private void OnDisable()
    {
        _closeBtn?.onClick.RemoveAllListeners();
    }

    public void Setup()
    {
    }

    private void handleCloseButtonClicked()
    {
        onCloseCliced?.Invoke();
    }
}
