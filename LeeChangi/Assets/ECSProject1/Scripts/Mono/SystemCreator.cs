using Sampel1;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Sample1
{
    public class SystemCreator : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            World.DefaultGameObjectInjectionWorld.CreateSystemManaged(typeof(Version2Systems));
        }

        
    }
}
