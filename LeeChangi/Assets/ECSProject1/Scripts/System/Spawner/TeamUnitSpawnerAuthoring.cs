using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using System;
using Unity.Rendering;
using Sample1;

[Serializable]
public struct TeamUnitSerialData
{
    public int TeamIndex;
    public int Count;
    public Color color;
    public float3 position;

    public float Speed;
}



public class TeamUnitSpawnerAuthoring : UnityEngine.MonoBehaviour
{
    public TeamUnitSerialData[] settings;
    public GameObject prefab;
    public GameObject parent;
    public class TeamUnitSpawnerBaker : Baker<TeamUnitSpawnerAuthoring>
    {
        public override void Bake(TeamUnitSpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            if (authoring.settings == null || authoring.settings.Length == 0) { return; }

            var builder = new BlobBuilder(Unity.Collections.Allocator.Temp);
            ref var units = ref builder.ConstructRoot<TeamUnits>();

            var arrayBuilder = builder.Allocate(ref units.units, authoring.settings.Length);

            for (var i = 0; i < authoring.settings.Length; i++)
            {
                var setting = authoring.settings[i];
                arrayBuilder[i] = new TeamUnit
                {
                    Count = setting.Count,
                    color = setting.color,
                    position = setting.position,
                    TeamIndex = setting.TeamIndex,
                };
            }

            

            var result = builder.CreateBlobAssetReference<TeamUnits>(Allocator.Persistent);
            builder.Dispose();
            AddComponent(entity ,new TeamUnitSpawnSet
            {
                teamSetting = result,
                baseEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                parentEntity = GetEntity(authoring.parent, TransformUsageFlags.Dynamic),
            });
            
        }
    }
}

public struct TeamUnit
{
    public int TeamIndex;
    public int Count;
    public Color color;
    public float3 position;
}

struct TeamUnits
{
    public BlobArray<TeamUnit> units;
}


struct TeamUnitSpawnSet : IComponentData
{
    public BlobAssetReference<TeamUnits> teamSetting;
    public Entity baseEntity;
    public Entity parentEntity;
}

[DisableAutoCreation]
[BurstCompile]
public partial struct TeamUnitSpawnerSystem : ISystem
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

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        for (var i = 0; i < length; i++)
        {
            var unit = setting.units[i];
            var crateUnits = CollectionHelper.CreateNativeArray<Entity>(unit.Count, Allocator.Temp);
            ecb.Instantiate(spawnSet.baseEntity, crateUnits);
            for (var j = 0; j < unit.Count; j++)
            {
                ecb.SetComponent(crateUnits[j], new LocalTransform
                {
                    Position = unit.position + new float3(random.NextFloat(-2, 2), 0, random.NextFloat(-2, 2)),
                    Rotation = quaternion.identity,
                    Scale = 1,
                });
                //ecb.SetComponent(crateUnits[j], new URPMaterialPropertyBaseColor
                //{
                //    Value = (Vector4)unit.color,
                //});
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
//public partial struct TeamUnitSpawnerJob : IJobEntity
//{
//	void Execute(ref LocalTransform transform, in TeamUnitSpawnSet value)
//	{
//	}
//}