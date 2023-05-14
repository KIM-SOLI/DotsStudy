using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeReferences;
using Unity.Entities;
using Unity.Scenes;
using UnityEngine;

namespace Sample1
{

    public interface IGetBakedSystem
    {
        public Type GetSystemType();
    }

    public class SubSceneWithSystem : SubScene
    {
        [SerializeField][ClassExtendsAttribute(typeof(IGetBakedSystem))] List<ClassTypeReference> initializedSystems;

        [SerializeField] public IGetBakedSystem[] onAwakeSystems;
        private List<SystemHandle> onAwakeSystemHandles;

        private void Start()
        {
            if (Application.isPlaying)
            {
                Init();
                LoadSystem();
            }
            
        }


     

        private void Init()
        {
            if (initializedSystems != null)
            {
                var systems = new List<IGetBakedSystem>();
                foreach (var item in initializedSystems)
                {
                    if (item == null || item.Type == null)
                    {
                        continue;
                    }
                    var constructor = item.Type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, Array.Empty<ParameterModifier>());
                    if (constructor?.Invoke(Array.Empty<object>()) is IGetBakedSystem system)
                    {
                        systems.Add(system);
                    }

                }

                onAwakeSystems = systems.ToArray();
            }
        }


        void LoadSystem()
        {
            onAwakeSystemHandles = new List<SystemHandle>();
            var world = World.DefaultGameObjectInjectionWorld;
            {
                if (onAwakeSystems != null)
                {
                    //SimulationSystemGroup orCreateSystemManaged2 = world.GetOrCreateSystemManaged<SimulationSystemGroup>();
                    //foreach(var system in onAwakeSystems)
                    //{
                    //    var handle = (world.GetOrCreateSystem(system.GetSystemType()));
                    //    orCreateSystemManaged2.AddSystemToUpdateList(handle);
                    //    onAwakeSystemHandles.Add(handle);
                    //}
                    DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world,onAwakeSystems.Select((x) => x.GetSystemType()));
                }
            }
        }

      

    }
}
