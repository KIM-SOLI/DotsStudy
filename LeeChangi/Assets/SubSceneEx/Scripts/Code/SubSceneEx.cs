using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using UnityEditor;
using UnityEditor.Experimental;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using File = System.IO.File;
using Hash128 = Unity.Entities.Hash128;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
#endif
#pragma warning disable 649

namespace Unity.Scenes
{
    [Serializable]
    public abstract class IGetBakedSystem
    {
        public abstract Type GetSystemType();
    }

    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class SubSceneEx : SubScene
    {
        [SerializeField] public IGetBakedSystem[] onAwakeSystems;

        unsafe protected void LoadSystem()
        {

            var world = World.DefaultGameObjectInjectionWorld;
            {
                if (onAwakeSystems != null)
                {

                    DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world,
                        onAwakeSystems.Select((x) => x.GetSystemType()));


                    //foreach (var system in onAwakeSystems)
                    //{
                    //    var handle = world.Unmanaged.GetOrCreateUnmanagedSystem(system.GetSystemType());
                    //    world.Unmanaged.ResolveSystemState(handle);

                    //}
                }
            }
        }
    }
}
