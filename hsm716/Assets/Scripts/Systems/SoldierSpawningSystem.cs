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

        URPMaterialPropertyBaseColor RandomColor()
        {
            hue = (hue + 0.618034005f) % 1;
            var color = UnityEngine.Color.HSVToRGB(hue, 1.0f, 1.0f);
            return new URPMaterialPropertyBaseColor { Value = (UnityEngine.Vector4)color };
        }

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var vehicles = CollectionHelper.CreateNativeArray<Entity>(100, Allocator.Temp);
        ecb.Instantiate(config.TankPrefab, vehicles);
        
        var queryMask = m_BaseColorQuery.GetEntityQueryMask();

        foreach (var vehicle in vehicles)
        {
            ecb.SetComponentForLinkedEntityGroup(vehicle, queryMask, RandomColor());

            ecb.SetComponent(vehicle, new LocalTransform
            {
                Position = new float3(random.NextInt(-200, 200), 0, random.NextInt(-200, 200)),
                Rotation = quaternion.identity,
                Scale = 1f
            });
            ecb.AddComponent(vehicle,new EnemyTag());
        }
        vehicles.Dispose();


        vehicles = CollectionHelper.CreateNativeArray<Entity>(5, Allocator.Temp);
        ecb.Instantiate(config.MeSoldierPrefab, vehicles);

        foreach (var vehicle in vehicles)
        {
            ecb.SetComponent(vehicle, new LocalTransform
            {
                Position = new float3(random.NextInt(-5, 5), 0, random.NextInt(-5, 5)),
                Rotation = quaternion.identity,
                Scale = 1f
            });
        }
        vehicles.Dispose();



        state.Enabled = false;
    }
}