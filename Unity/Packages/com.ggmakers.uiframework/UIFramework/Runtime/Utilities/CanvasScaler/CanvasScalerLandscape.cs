using UnityEngine;
using UnityEngine.UI;

namespace GGMakers.UIFramework.Utilities
{
    public class CanvasScalerLandscape : MonoBehaviour
    {
        [SerializeField]
        public Vector2 ReferenceRatio;

        private CanvasScaler _scaler;
        private int _width;
        private int _height;

        private void Awake()
        {
            UpdateRatio();
        }

        private void UpdateRatio()
        {
            if (_scaler == null)
            {
                _scaler = GetComponent<CanvasScaler>();
            }

            float ratio = (float)Screen.width / Screen.height;
            float designRatio = ReferenceRatio.x / ReferenceRatio.y;
            _scaler.matchWidthOrHeight = ratio > designRatio ? 1 : 0;
        }

        private void Update()
        {
            if (UIManager.Instance == null)
                return;

            if (_width != Screen.width || _height != Screen.height)
            {
                _width = Screen.width;
                _height = Screen.height;

                UpdateRatio();
            }
        }
    }
}
