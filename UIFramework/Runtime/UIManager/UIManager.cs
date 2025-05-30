﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GGMakers.UIFramework
{
    public static class UILayer
    {
        public static int Game = 0;
        public static int Main = 1;
        public static int Overlay = 2;
    }

    public class UICachedUnit
    {
        public string prefab;
        public ResourceRequest resourceRequest;
        public GameObject instanceUI;
        public UnityEngine.Object asset;
    }

    public partial class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        public Camera CameraUI
        {
            get { return _cameraUI; }
            set { _cameraUI = value; }
        }

        public EventSystem EventSystem
        {
            get { return _eventSystem; }
            set { _eventSystem = value; }
        }

        public Canvas GameCanvas
        {
            get { return _gameCanvas; }
            set { _gameCanvas = value; }
        }

        public Canvas MainCanvas
        {
            get { return _mainCanvas; }
            set { _mainCanvas = value; }
        }

        public Canvas OverlayCanvas
        {
            get { return _overlayCanvas; }
            set { _overlayCanvas = value; }
        }

        public List<Transform> UILayers
        {
            get { return _UILayers; }
            set { _UILayers = value; }
        }

        public RectTransform GameRect
        {
            get { return _gameRect; }
            set { _gameRect = value; }
        }

        public RectTransform MainRect
        {
            get { return _mainRect; }
            set { _mainRect = value; }
        }

        public RectTransform OverlayRect
        {
            get { return _overlayRect; }
            set { _overlayRect = value; }
        }

        [HideInInspector]
        public string BaseUIPath
        {
            get { return _baseUIPath; }
            set { _baseUIPath = value; }
        }

        public string GetUIPath(string uiPrefab)
        {
            return string.Format("{0}/{1}", _baseUIPath, uiPrefab);
        }

        [SerializeField] string _baseUIPath;
        [SerializeField] Camera _cameraUI;
        [SerializeField] EventSystem _eventSystem;
        [SerializeField] List<Transform> _UILayers;
        [SerializeField] RectTransform _gameRect, _mainRect, _overlayRect;
        [SerializeField] Canvas _gameCanvas, _mainCanvas, _overlayCanvas;

        private Dictionary<string, UICachedUnit> cachedUIData = new Dictionary<string, UICachedUnit>();

        private void OnDestroy()
        {
            try
            {
                ReleaseUIInstances();
            }
            catch (System.Exception ex)
            {
                Debug.LogErrorFormat("Destroy UIManager with exception: {0}\n{1}", ex.Message, ex.StackTrace);
            }

            Instance = null;
        }

        public void SetUIInteractable(bool active)
        {
            EventSystem.gameObject.SetActive(active);
        }

        public bool IsUIInteractable
        {
            get
            {
                return EventSystem.gameObject.activeInHierarchy;
            }
        }

        public Rect GetScreenRect(RectTransform rectTransform, bool screenOverlay = false)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            float xMin = float.PositiveInfinity, xMax = float.NegativeInfinity, yMin = float.PositiveInfinity, yMax = float.NegativeInfinity;
            for (int i = 0; i < 4; ++i)
            {
                Vector3 screenCoord = RectTransformUtility.WorldToScreenPoint(screenOverlay ? null : CameraUI, corners[i]);
                if (screenCoord.x < xMin) xMin = screenCoord.x;
                if (screenCoord.x > xMax) xMax = screenCoord.x;
                if (screenCoord.y < yMin) yMin = screenCoord.y;
                if (screenCoord.y > yMax) yMax = screenCoord.y;
                corners[i] = screenCoord;
            }
            Rect result = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

            return result;
        }

        public void PreloadUI(string path, bool createInstance = false)
        {
            UICachedUnit cached;
            if (!cachedUIData.TryGetValue(path, out cached))
            {
                //Debug.Log("[GameUI]Preload prefab: " + path);

                cached = new UICachedUnit();
                cached.prefab = path;
                cached.resourceRequest = Resources.LoadAsync(GetUIPath(path));
                cachedUIData.Add(path, cached);
            }

            if (createInstance && cached.instanceUI == null)
            {
                StartCoroutine(InstancetiateUI(cached));
            }
        }

        public bool IsUILoaded(string path, bool creatInstance = false)
        {
            UICachedUnit cached;
            if (cachedUIData.TryGetValue(path, out cached))
            {
                return !creatInstance || cached.instanceUI != null;
            }

            return false;
        }

        public GameObject GetLoadedUI(string path)
        {
            UICachedUnit cached;
            if (cachedUIData.TryGetValue(path, out cached))
            {
                return cached.instanceUI;
            }

            return null;
        }

        public bool CheckAndLoadUI(string path, bool loadIfNotExist)
        {
            UICachedUnit cached;
            if (!cachedUIData.TryGetValue(path, out cached) && loadIfNotExist)
            {
                cached = new UICachedUnit();
                cached.prefab = path;
                cached.resourceRequest = Resources.LoadAsync(GetUIPath(path));
                cachedUIData.Add(path, cached);
            }

            if (cached != null && cached.instanceUI != null)
                return true;

            if (loadIfNotExist && cached.instanceUI == null)
            {
                StartCoroutine(InstancetiateUI(cached));
            }

            return false;
        }

        public bool IsVisible(UIController controller)
        {
            if (controller == null)
                return false;

            for (int i = UILayers.Count - 1; i >= 0; i--)
            {
                if (UILayers[i].gameObject.activeInHierarchy && VisibleUIs[i].ActiveControllers.Count > 0)
                    return controller.transform.parent == UILayers[i] && VisibleUIs[controller.UILayer].ActiveControllers.Find(delegate (ActiveControllerData visible)
                    {
                        return visible.Controller == controller;
                    }) != null;
            }

            return false;
        }

        public bool IsVisibleOnTop(UIController controller)
        {
            if (controller == null)
                return false;

            for (int i = UILayers.Count - 1; i >= 0; i--)
            {
                if (UILayers[i].gameObject.activeInHierarchy && VisibleUIs[i].ActiveControllers.Count > 0)
                {
                    var count = VisibleUIs[controller.UILayer].ActiveControllers.Count;
                    var topUI = VisibleUIs[controller.UILayer].ActiveControllers[count - 1];
                    return controller.transform.parent == UILayers[i] && topUI.Controller == controller;
                }
            }

            return false;
        }

        public bool IsActive(UIController controller)
        {
            return controller.gameObject.activeInHierarchy && VisibleUIs[controller.UILayer].ActiveControllers.Find(delegate (ActiveControllerData visible)
            {
                return visible.Controller == controller;
            }) != null;
        }

        public bool IsActiveOnTop(UIController controller)
        {
            return VisibleUIs[controller.UILayer].ActiveControllers[VisibleUIs[controller.UILayer].ActiveControllers.Count - 1].Controller == controller;
        }

        IEnumerator InstancetiateUI(UICachedUnit cached, int layer = UI_LAYER_NORMAL)
        {
            yield return cached.resourceRequest;

            var RootUI = UILayers[layer];
            var objInstance = Instantiate(cached.resourceRequest.asset) as GameObject;
            objInstance.SetActive(false);
            objInstance.transform.SetParent(RootUI, false);

            RectTransform rectTrans = objInstance.GetComponent<RectTransform>();
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
            rectTrans.offsetMin = rectTrans.offsetMax = Vector2.zero;

            yield return null;

            cached.instanceUI = objInstance;
        }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;

            VisibleUIs = new List<UIVisibleData>();
            for (int i = 0; i < UILayers.Count; i++)
            {
                VisibleUIs.Add(new UIVisibleData() { ActiveControllers = new List<ActiveControllerData>() });

                for (int j = 0; j < UILayers[i].childCount; j++)
                {
                    var instanceUI = UILayers[i].GetChild(j).GetComponent<UIController>();
                    if (instanceUI != null && instanceUI.gameObject.activeInHierarchy)
                    {
                        var activeData = new ActiveControllerData() { Controller = instanceUI };
                        VisibleUIs[i].ActiveControllers.Add(activeData);

                        EnableUI(activeData, i);
                        instanceUI.UIStart();
                    }
                }
            }
        }

        IEnumerator CaptureOverlayBG(RawImage overlayBG, RenderTexture overlayTexture, System.Action cb, int layer = UI_LAYER_NORMAL)
        {
            yield return new WaitForEndOfFrame();

            if (Camera.allCameras.Length > 1)
            {
                var activeCameras = new List<Camera>(Camera.allCameras);
                activeCameras.Remove(CameraUI);

                activeCameras.Sort(delegate (Camera x, Camera y)
                {
                    return x.depth.CompareTo(y.depth);
                });

                foreach (var c in activeCameras)
                {
                    c.enabled = false;
                    c.targetTexture = overlayTexture;
                    c.Render();

                    c.enabled = true;
                    c.targetTexture = null;
                }
            }

            var states = new bool[UILayers.Count];
            for (int i = 0; i < UILayers.Count; i++)
            {
                if (i > layer)
                {
                    states[i] = UILayers[i].gameObject.activeInHierarchy;
                    UILayers[i].gameObject.SetActive(false);
                }
            }

            var RootUI = UILayers[layer];
            bool active = RootUI.gameObject.activeInHierarchy;
            RootUI.gameObject.SetActive(true);
            CameraUI.enabled = false;
            CameraUI.targetTexture = overlayTexture;
            CameraUI.Render();
            RootUI.gameObject.SetActive(active);

            for (int i = 0; i < UILayers.Count; i++)
            {
                if (i > layer)
                {
                    UILayers[i].gameObject.SetActive(states[i]);
                }
            }

            CameraUI.targetTexture = null;
            CameraUI.enabled = true;

            overlayBG.gameObject.SetActive(true);
            cb();
        }

        public IEnumerator CaptureUIScreen(RenderTexture overlayTexture, int layer = UI_LAYER_NORMAL)
        {
            yield return new WaitForEndOfFrame();

            if (Camera.allCameras.Length > 1)
            {
                var activeCameras = new List<Camera>(Camera.allCameras);
                activeCameras.Remove(CameraUI);

                activeCameras.Sort(delegate (Camera x, Camera y)
                {
                    return x.depth.CompareTo(y.depth);
                });

                foreach (var c in activeCameras)
                {
                    c.enabled = false;
                    c.targetTexture = overlayTexture;
                    c.Render();

                    c.enabled = true;
                    c.targetTexture = null;
                }
            }

            var states = new bool[UILayers.Count];
            for (int i = 0; i < UILayers.Count; i++)
            {
                if (i > layer)
                {
                    states[i] = UILayers[i].gameObject.activeInHierarchy;
                    UILayers[i].gameObject.SetActive(false);
                }
            }

            var RootUI = UILayers[layer];
            bool active = RootUI.gameObject.activeInHierarchy;
            RootUI.gameObject.SetActive(true);
            CameraUI.enabled = false;
            CameraUI.targetTexture = overlayTexture;
            CameraUI.Render();
            RootUI.gameObject.SetActive(active);

            for (int i = 0; i < UILayers.Count; i++)
            {
                if (i > layer)
                {
                    UILayers[i].gameObject.SetActive(states[i]);
                }
            }

            CameraUI.targetTexture = null;
            CameraUI.enabled = true;
        }
    }
}

