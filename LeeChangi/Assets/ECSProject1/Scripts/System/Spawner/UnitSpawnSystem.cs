using PlasticGui;
using System;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sample1
{


    public class UnitSpawnSystemAuthoring : IGetBakedSystem
    {
        public UnitSpawnSystemAuthoring() { }
        public Type GetSystemType()
        {
            return typeof(UnitSpawnSystem);
        }
    }

    [UpdateAfter(typeof(TransformSystemGroup))]
    [DisableAutoCreation]
    [BurstCompile]
    public partial struct UnitSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TeamUnitSpawnSet>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
           
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var random = Unity.Mathematics.Random.CreateFromIndex(1234);

            var spawnSet = SystemAPI.GetSingleton<TeamUnitSpawnSet>();
            ref var setting = ref spawnSet.teamSetting.Value;
            var length = setting.units.Length;

            var ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

            for (var i = 0; i < length; i++)
            {
                var unit = setting.units[i];
                var crateUnits = CollectionHelper.CreateNativeArray<Entity>(unit.Count, Allocator.Temp);
                
                ecb.Instantiate(spawnSet.baseEntity, crateUnits);
                for (var j = 0; j < unit.Count; j++)
                {
                    //ecb.AddComponent<PostTransformMatrix>(crateUnits[j]);
                    ecb.SetComponent(crateUnits[j], new LocalTransform
                    {
                        Position = unit.position + new float3(random.NextFloat(-2, 2), 0, random.NextFloat(-2, 2)),
                        Rotation = quaternion.identity,
                        Scale = 1,
                    });
                    
                    ecb.SetComponent(crateUnits[j], new URPMaterialPropertyBaseColor
                    {
                        Value = (Vector4)unit.color,
                    });
                    ecb.SetComponent(crateUnits[j], new TeamUnitComponentData
                    {
                        TeamIndex = unit.TeamIndex,
                    });
                }
            }
            state.Enabled = false;
        }
    }



    //[BurstCompile]
    //public partial struct SampleJob : IJobChunk
    //{
    //    public EntityTypeHandle entityHandle;
    //    public EntityCommandBuffer.ParallelWriter writer;

    //    [BurstCompile]
    //    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    //    {
    //        NativeArray<Entity> entities =
    //               chunk.GetNativeArray(entityHandle);
           
    //        var enumerator = new ChunkEntityEnumerator(
    //            useEnabledMask,
    //            chunkEnabledMask,
    //            chunk.Count);

    //        while (enumerator.NextEntityIndex(out var index))
    //        {
    //            var entity = entities[index];
    //        }
    //    }
    //}

}