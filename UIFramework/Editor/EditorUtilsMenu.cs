using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using GGMakers.UIFramework;
using GGMakers.UIFramework.Utilities;

namespace GGMakers.Editor
{
    public static class UtilMenu
    {
        [MenuItem("GGMakers/UIFramework/Utililies/Clear Player Pref", false, 20)]
        public static void ClearPlayerPref()
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Cleared player preferences!");
        }

        [MenuItem("GGMakers/UIFramework/Utililies/Clear Persistant Data", false, 20)]
        public static void ClearPersistantData()
        {
            Debug.Log("Persistant data located at :" + Application.persistentDataPath);
            DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
            dir.Delete(true);
            Debug.Log("Deleted player persistant data!");
        }

        [MenuItem("GGMakers/UIFramework/Utililies/Clear temporary Data", false, 20)]
        public static void ClearTempData()
        {
            Debug.Log("Temporary data located at :" + Application.temporaryCachePath);
            DirectoryInfo dir = new DirectoryInfo(Application.temporaryCachePath);
            dir.Delete(true);
            Debug.Log("Deleted player temporary data!");
        }

        [MenuItem("GGMakers/UIFramework/Utililies/Clear All", false, 12)]
        public static void ClearAll()
        {
            ClearPlayerPref();
            ClearPersistantData();
            ClearTempData();
            Caching.ClearCache();
        }

        [MenuItem("GGMakers/UIFramework/Utililies/UnloadAllUnusedAssets", false, 20)]
        public static void UnloadAllUnusedAsset()
        {
            Debug.Log("Unload all unused assets ");

            Resources.UnloadUnusedAssets();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        [MenuItem("GGMakers/UIFramework/Create UIManager", false, 30)]
        public static void CreateUIManager()
        {
            //Clean up current scene
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            var rootObjects = activeScene.GetRootGameObjects();
            foreach (var obj in rootObjects)
            {
                if (obj.name.Equals("UIManager"))
                {
                    Object.DestroyImmediate(obj);
                }
            }

            LayerMaskEx.CreateLayers(new string[] { "Game", "UI", "ScreenOverlay" });
            LayerMaskEx.CreateSortingLayers(new string[] { "Game", "UI", "ScreenOverlay" });

            var uiLayer = LayerMask.NameToLayer("UI");
            var screenOverlayLayer = LayerMask.NameToLayer("ScreenOverlay");
            var gameLayer = LayerMask.NameToLayer("Game");

            //Create UIManager
            var uiManager = new GameObject("UIManager", typeof(UIManager)).GetComponent<UIManager>();
            uiManager.gameObject.layer = uiLayer;
            uiManager.BaseUIPath = "UIPrefabs";
            uiManager.UnlitTextureColorMaterial = Resources.Load<Material>("Materials/unlit-texture-color");

            //Create UI camera
            var uiCamera = new GameObject("UICamera", typeof(Camera)).GetComponent<Camera>();
            uiCamera.gameObject.layer = uiLayer;
            uiCamera.gameObject.AddComponent<AudioListener>();
            uiCamera.transform.SetParent(uiManager.transform, false);
            uiCamera.renderingPath = RenderingPath.Forward;
            uiCamera.clearFlags = CameraClearFlags.Depth;
            uiCamera.orthographic = true;
            uiCamera.orthographicSize = 9.6f;
            uiCamera.nearClipPlane = -100f;
            uiCamera.farClipPlane = 100f;
            uiCamera.useOcclusionCulling = false;
            uiCamera.allowHDR = false;
            uiCamera.allowMSAA = false;
            uiCamera.cullingMask = 1 << uiLayer | 1 << screenOverlayLayer;
            uiCamera.depth = 100;
            uiManager.CameraUI = uiCamera;

            //Create EventSystem
            var eventSystem = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem)).GetComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.gameObject.layer = uiLayer;
            eventSystem.transform.SetParent(uiManager.transform, false);
            eventSystem.gameObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            uiManager.EventSystem = eventSystem;

            //Create MainCanvas
            var mainCanvas = new GameObject("MainCanvas", typeof(Canvas)).GetComponent<Canvas>();
            mainCanvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            mainCanvas.transform.SetParent(uiManager.transform, false);
            mainCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            mainCanvas.pixelPerfect = false;
            mainCanvas.worldCamera = uiCamera;
            mainCanvas.planeDistance = 0f;
            mainCanvas.gameObject.layer = uiLayer;
            uiManager.MainCanvas = mainCanvas;

            //MainCanvas scaler
            var canvasScaler = mainCanvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1080f, 1920f);
            canvasScaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasScaler.matchWidthOrHeight = 1f;
            canvasScaler.referencePixelsPerUnit = 100f;

            var mainPortrailScaler = mainCanvas.gameObject.AddComponent<CanvasScalerPortrait>();
            mainPortrailScaler.ReferenceRatio = new Vector2(9, 16);

            //MainCanvas rect
            var mainCanvasRect = new GameObject("MainRect", typeof(RectTransform)).GetComponent<RectTransform>();
            mainCanvasRect.SetParent(mainCanvas.transform, false);
            mainCanvasRect.anchorMin = Vector2.zero;
            mainCanvasRect.anchorMax = Vector2.one;
            mainCanvasRect.offsetMin = Vector2.zero;
            mainCanvasRect.offsetMax = Vector2.zero;
            mainCanvasRect.gameObject.layer = uiLayer;
            mainCanvas.sortingLayerName = "UI";
            uiManager.MainRect = mainCanvasRect;

            //Create OverlayCanvas
            var overlayCanvas = new GameObject("OverlayCanvas", typeof(Canvas)).GetComponent<Canvas>();
            overlayCanvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            overlayCanvas.transform.SetParent(uiManager.transform, false);
            overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            overlayCanvas.pixelPerfect = false;
            overlayCanvas.planeDistance = 0f;
            overlayCanvas.gameObject.layer = screenOverlayLayer;
            overlayCanvas.sortingLayerName = "ScreenOverlay";
            uiManager.OverlayCanvas = overlayCanvas;

            //OverlayCanvas scaler
            var overlayCanvasScaler = overlayCanvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            overlayCanvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            overlayCanvasScaler.referenceResolution = new Vector2(1080f, 1920f);
            overlayCanvasScaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            overlayCanvasScaler.matchWidthOrHeight = 1f;
            overlayCanvasScaler.referencePixelsPerUnit = 100f;

            var overlayPortrailScaler = overlayCanvas.gameObject.AddComponent<CanvasScalerPortrait>();
            overlayPortrailScaler.ReferenceRatio = new Vector2(9, 16);

            //OverlayCanvas rect
            var overlayCanvasRect = new GameObject("OverlayRect", typeof(RectTransform)).GetComponent<RectTransform>();
            overlayCanvasRect.SetParent(overlayCanvas.transform, false);
            overlayCanvasRect.anchorMin = Vector2.zero;
            overlayCanvasRect.anchorMax = Vector2.one;
            overlayCanvasRect.offsetMin = Vector2.zero;
            overlayCanvasRect.offsetMax = Vector2.zero;
            overlayCanvasRect.gameObject.layer = screenOverlayLayer;
            uiManager.OverlayRect = overlayCanvasRect;

            //Create GameCanvas
            var gameCanvas = new GameObject("GameCanvas", typeof(Canvas)).GetComponent<Canvas>();
            gameCanvas.gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            gameCanvas.transform.SetParent(uiManager.transform, false);
            gameCanvas.transform.SetSiblingIndex(2);
            gameCanvas.renderMode = RenderMode.WorldSpace;
            gameCanvas.pixelPerfect = false;
            mainCanvas.worldCamera = uiCamera;
            gameCanvas.planeDistance = 0f;
            gameCanvas.gameObject.layer = gameLayer;
            gameCanvas.sortingLayerName = "Game";
            uiManager.GameCanvas = gameCanvas;

            //GameCanvas scaler
            var gameCanvasScaler = gameCanvas.gameObject.AddComponent<UnityEngine.UI.CanvasScaler>();
            gameCanvasScaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameCanvasScaler.referenceResolution = new Vector2(1080f, 1920f);
            gameCanvasScaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            gameCanvasScaler.matchWidthOrHeight = 1f;
            gameCanvasScaler.referencePixelsPerUnit = 100f;

            var gamePortrailScaler = gameCanvasScaler.gameObject.AddComponent<CanvasScalerPortrait>();
            gamePortrailScaler.ReferenceRatio = new Vector2(9, 16);

            //Update GameCanvas size
            var worlSpaceCanvasScaler = gameCanvas.gameObject.AddComponent<WorldSpaceCanvasScaler>();
            worlSpaceCanvasScaler.UICanvasRect = mainCanvas.GetComponent<RectTransform>();
            worlSpaceCanvasScaler.UpdateSize();

            //GameCanvas Rect
            var gameCanvasRect = new GameObject("GameRect", typeof(RectTransform)).GetComponent<RectTransform>();
            gameCanvasRect.SetParent(gameCanvas.transform, false);
            gameCanvasRect.anchorMin = Vector2.zero;
            gameCanvasRect.anchorMax = Vector2.one;
            gameCanvasRect.offsetMin = Vector2.zero;
            gameCanvasRect.offsetMax = Vector2.zero;
            gameCanvasRect.gameObject.layer = gameLayer;
            uiManager.GameRect = gameCanvasRect;

            //Set layers
            uiManager.UILayers = new List<Transform>() { gameCanvasRect.transform, mainCanvasRect.transform, overlayCanvasRect.transform };

#if UNITY_2022_1_OR_NEWER
            gameCanvas.vertexColorAlwaysGammaSpace = true;
            mainCanvas.vertexColorAlwaysGammaSpace = true;
            overlayCanvas.vertexColorAlwaysGammaSpace = true;
#endif

            //Add safe area simulator
            uiManager.gameObject.AddComponent<SafeAreaSimulator>();
        }
    }
}