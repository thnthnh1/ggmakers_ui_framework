using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGMakers.UIFramework.Utilities
{
    public class WorldSpaceCanvasScaler : MonoBehaviour
    {
        [SerializeField]
        public RectTransform UICanvasRect;

        private RectTransform _rect;

        private void Start()
        {
            UpdateSize();
        }

        public void UpdateSize()
        {
            _rect = GetComponent<RectTransform>();

            StartCoroutine(snapSizeProcess());
        }

        private bool _updatingSize = false;
        private WaitForEndOfFrame _yieldEndOfFrame = new WaitForEndOfFrame();
        private IEnumerator snapSizeProcess()
        {
            _updatingSize = true;

            yield return _yieldEndOfFrame;

            _rect.sizeDelta = UICanvasRect.sizeDelta;
            _rect.localScale = UICanvasRect.localScale;

            _updatingSize = false;
        }

        private void Update()
        {
            if (_rect.sizeDelta != UICanvasRect.sizeDelta && !_updatingSize)
            {
                UpdateSize();
            }
        }
    }
}