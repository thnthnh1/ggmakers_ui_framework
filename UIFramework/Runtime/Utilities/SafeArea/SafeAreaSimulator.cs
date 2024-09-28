using UnityEngine;
using System;

namespace GGMakers.UIFramework.Utilities
{
    public class SafeAreaSimulator : MonoBehaviour
    {
        [SerializeField]
        private SafeArea.SimDevice _device;

        void Awake ()
        {
            if (!Application.isEditor)
                Destroy (this);

            ToggleSafeArea();
        }

        //void Update ()
        //{
        //    ToggleSafeArea ();
        //}

        /// <summary>
        /// Toggle the safe area simulation device.
        /// </summary>
        void ToggleSafeArea()
        {
            SafeArea.Sim = _device;
            Debug.LogFormat ("Switched to sim device {0}", _device);
        }
    }
}
