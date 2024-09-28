using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGMakers.UIFramework;

public class SceneController : MonoBehaviour
{
    private GameUI _gameUI;

    private void Start()
    {
        _gameUI = UIManager.Instance.ShowUIOnTop<GameUI>("GameUI", true, UILayer.Game);
        _gameUI.onShowClicked = showDemoUI;
        _gameUI.Setup();
    }

    private void showDemoUI()
    {
        var demoUI = UIManager.Instance.ShowUIOnTop<DemoUI>("DemoUI", true, UILayer.Main);
        demoUI.onCloseCliced = () => {
            UIManager.Instance.ReleaseUI(demoUI, true);
        };
        demoUI.Setup();
    }
}
