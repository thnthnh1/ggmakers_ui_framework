# UIFramework

UIFramework is a framework based on UGUI to help you setup, create and implement the UI in game easily.

## Installation

Find the manifest.json file in the Packages folder of your project and edit it to look like this:

```
{
  "dependencies": {
    "com.athena.uiframework": "https://github.com/thnthnh1/athena_framework.git#v1.0.1",
    ...
  }
}
```

To update the package, change suffix #{version} to the target version.

```
 "com.athena.uiframework": "https://github.com/thnthnh1/athena_framework.git#v1.0.2"
```


## Usage

1. Create UIManager in the scene by select the command on the toolbar:
```
Athena > UIFramework > Create UIManager
```
<!-- CREATE UIMANAGER -->
<br />
<div align="left">
  <a href="">
    <img src="Images/create_ui_manager.png" alt="Logo" width="561" height="137">
  </a>
</div>


2. Create new class inheirits UIController for each UI that you want to implement.

```C#
using Athena.UIFramework;

public class DemoUI : UIController
{
    public void Setup()
    {

    }
}
```

3. Create new UI prefab at Resources/UIPrefabs/ and attach DemoUI to this prefab.
<!-- CREATE PREFAB -->
<br />
<div align="left">
  <a href="">
    <img src="Images/create_prefab.png" alt="Logo" width="660" height="467">
  </a>
</div>


4. Call UIManager's instance to show DemoUI

```C#
using UnityEngine;
using Athena.UIFramework;

public class SceneController : MonoBehaviour
{
    private void Start()
    {
        var demoUI = UIManager.Instance.ShowUIOnTop<DemoUI>("DemoUI", true, UILayer.Main);
        demoUI.Setup();
    }
}

```

## Note

There are 3 pre-defined layers to use and they follow this order:
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