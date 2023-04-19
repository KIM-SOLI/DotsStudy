using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample1
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] Canvas canvas;
        private void Awake()
        {
            canvas.worldCamera = WorldUICamera.uiCamera;
        }
    }
}
