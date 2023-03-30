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
        Type GetSystemType();
    }

    public class SubSceneWithSystem : SubScene
    {
        [SerializeField] [ClassExtendsAttribute(typeof(IGetBakedSystem))] List<ClassTypeReference> initializedSystems;

        [SerializeField] public IGetBakedSystem[] onAwakeSystems;

        private void Start()
        {
            Init();
            LoadSystem();
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

                    var constructors = item.Type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                    var constructors2 = item.Type.GetConstructors(BindingFlags.CreateInstance);

                    //constructors.orEach((x)=> Debug.Log(x)) ;
                    //Debug.Log(constructors)

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

            var world = World.DefaultGameObjectInjectionWorld;
            {
                if (onAwakeSystems != null)
                {

                    DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world,
                        onAwakeSystems.Select((x) => x.GetSystemType()));


                    
                }
            }
        }

    }
}
