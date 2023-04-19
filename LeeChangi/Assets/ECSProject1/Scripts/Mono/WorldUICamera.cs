using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sample1
{
    public class WorldUICamera : MonoBehaviour
    {
       [SerializeField] Canvas canvas;

        public static Camera uiCamera { get; private set; }
        private void Awake()
        {
            uiCamera =  canvas.worldCamera;
        }
    }
}
