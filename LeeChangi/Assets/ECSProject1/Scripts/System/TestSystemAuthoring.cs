using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Sample1
{
    
    public class TestSystemAuthoring : IGetBakedSystem
    {
        public TestSystemAuthoring(){}
        public Type GetSystemType()
        {
            return typeof(TestSystem);
        }
    }



    [DisableAutoCreation]
    [BurstCompile]
    public partial struct TestSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

        }
    }


}

