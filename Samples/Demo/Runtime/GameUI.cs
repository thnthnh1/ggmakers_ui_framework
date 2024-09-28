using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GGMakers.UIFramework;

public class GameUI : UIController
{
    public System.Action onShowClicked;

    [SerializeField]
    private Button _showBtn;

    private void OnEnable()
    {
        _showBtn?.onClick.AddListener(handleShowButtonClicked);
    }

    private void OnDisable()
    {
        _showBtn?.onClick.RemoveAllListeners();
    }

    public void Setup()
    {

    }

    private void handleShowButtonClicked()
    {
        onShowClicked?.Invoke();
    }
}
