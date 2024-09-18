using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Athena.UIFramework;

public class SceneController : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.ShowUIOnTop<DemoUI>("DemoUI", true, UILayer.Main);
    }
}
