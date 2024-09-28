using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGMakers.UIFramework.Utilities
{
    public class WorldSpaceCanvasScaler : MonoBehaviour
    {
        [SerializeField]
        public Canvas UICanvas;

        private RectTransform _rect;
        private Vector2 _uiCanvasSize;

        private void Start()
        {
            UpdateSize();
        }

        public void UpdateSize()
        {
            _rect = GetComponent<RectTransform>();
            _uiCanvasSize = UICanvas.GetComponent<RectTransform>().sizeDelta;

            StartCoroutine(snapSizeProcess());
        }

        private bool _updatingSize = false;
        private WaitForEndOfFrame _yieldEndOfFrame = new WaitForEndOfFrame();
        private IEnumerator snapSizeProcess()
        {
            _updatingSize = true;

            yield return _yieldEndOfFrame;

            _rect.sizeDelta = _uiCanvasSize;
            _rect.localScale = UICanvas.transform.localScale;

            _updatingSize = false;
        }

        private void Update()
        {
            if (_rect.sizeDelta != _uiCanvasSize && !_updatingSize)
            {
                UpdateSize();
            }
        }
    }
}