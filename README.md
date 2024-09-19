# UIFramework

UIFramework is a framework based on UGUI to help you setup, create and implement the UI in game easily.

## Installation

Find the manifest.json file in the Packages folder of your project and edit it to look like this:

```
{
  "dependencies": {
    "com.athena.uiframework": "https://github.com/thnthnh1/athena_framework.git",
    ...
  }
}
```

To update the package, change suffix #{version} to the target version.

```
 "com.athena.uiframework": "https://github.com/thnthnh1/athena_framework.git#v1.0.4"
```


## Usage

1. Create UIManager in the scene by select the command on the toolbar:
```
Athena > UIFramework > Create UIManager
```

![create_ui_manager](https://github.com/user-attachments/assets/e9d77dcc-e8c6-43a9-b2f3-cd0ffccf329c)

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

3. Create new UI prefab at folder:
```
Resources > UIPrefabs
```

4. Attach DemoUI.cs to the new UI prefab.

![create_prefab](https://github.com/user-attachments/assets/cd1d34ec-d6cd-4477-9393-89f573bb7190)

6. Call UIManager's instance to show DemoUI

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
