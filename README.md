# UIFramework

UIFramework is a framework based on UGUI to help you setup, create and implement the UI in game easily.

## Installation

Add this git repository to your Packages/manifest.json to install.

```JSON
{
  "dependencies": {
    "com.athena.uiframework": "https://github.com/thnthnh1/athena_framework.git#v1.0.1",
    ...
  }
}
```

## Usage

Create UIManager in the scene by select: Athena/UIFramework/Create UIManager on the toolbar

Create new class inheirit UIController for each UI that you want to implement.

```C#
using Athena.UIFramework;

public class DemoUI : UIController
{
    public void Setup()
    {

    }
}
```

Create new UI prefab at Resources/UIPrefabs/ and attach DemoUI to this prefab.

Call UIManager.Instance.ShowUI<T>(string path, bool isOverlay, int layer) to show DemoUI

```C#
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
```

There are 3 layers to show in UIFramework and they follow this order:
"Game" < "Main" < Overlay

```C#
public static class UILayer
{
    public static int Game = 0;
    public static int Main = 1;
    public static int Overlay = 2;
}
```

Attach SafeArea.cs to the UI prefab that you want to show in the device safe area.

Attach SafeAreaSimulator to UIManager game object if you want to check the safe area in Editor.