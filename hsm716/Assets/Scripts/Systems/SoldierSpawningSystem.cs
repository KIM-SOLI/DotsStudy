using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.UIElements;

[BurstCompile]
partial struct SoldierSpawningSystem : ISystem
{
    EntityQuery m_BaseColorQuery;
    EntityQuery m_WorldPosition;
    EntityQuery m_Enemies;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Config>();

        m_BaseColorQuery = state.GetEntityQuery(ComponentType.ReadOnly<URPMaterialPropertyBaseColor>());
        m_WorldPosition = state.GetEntityQuery(ComponentType.ReadOnly<LocalTransform>());
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {
        var config = SystemAPI.GetSingleton<Config>();

        var random = Unity.Mathematics.Random.CreateFromIndex(1234);
        var hue = random.NextFloat();

        URPMaterialPropertyBaseColor RandomColor(int index)
        {
            UnityEngine.Color color = UnityEngine.Color.HSVToRGB(1.0f, 1.0f, 1.0f);
            switch (index)
            {
                case 0:
                    color = UnityEngine.Color.HSVToRGB(1.0f, 0.3f, 1.0f);
                    break;
                case 1:
                    color = UnityEngine.Color.HSVToRGB(0.5f, 1.0f, 1.0f);
                    break;
            }

            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var vehicles = CollectionHelper.CreateNativeArray<Entity>(3000, Allocator.Temp);
        ecb.Instantiate(config.TankPrefab, vehicles);
        
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();

        foreach (var vehicle in vehicles)
        {
            ecb.SetComponentForLinkedEntityGroup(vehicle, queryMask, RandomColor(0));

            ecb.SetComponent(vehicle, new LocalTransform
            {
                Position = new float3(random.NextInt(-200, 200), 0.75f, random.NextInt(-200, 200)),
                Rotation = quaternion.identity,
                Scale = random.NextInt(1, 3)
            });
            ecb.AddComponent(vehicle,new EnemyTag() { dirChangeTime=-1f});
        }
        vehicles.Dispose();


        vehicles = CollectionHelper.CreateNativeArray<Entity>(5, Allocator.Temp);
        ecb.Instantiate(config.MeSoldierPrefab, vehicles);

        foreach (var vehicle in vehicles)
        {
            ecb.SetComponentForLinkedEntityGroup(vehicle, queryMask, RandomColor(1));

            ecb.SetComponent(vehicle, new LocalTransform
            {
                Position = new float3(random.NextInt(-5, 5), 0.75f, random.NextInt(-5, 5)),
                Rotation = quaternion.identity,
                Scale = 1f
            });
            ecb.AddComponent(vehicle, new MySoldierTag());
        }
        vehicles.Dispose();

        state.Enabled = false;
    }
}