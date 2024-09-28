using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGMakers.UIFramework;

public class SceneController : MonoBehaviour
{
    private void Start()
    {
        var demoUI = UIManager.Instance.ShowUIOnTop<DemoUI>("DemoUI", true, UILayer.Main);
        demoUI.Setup();
    }
}
