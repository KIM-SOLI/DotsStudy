using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

[BurstCompile]// This system should run after the transform system has been updated, otherwise the camera
partial struct LifeCycleSystem : ISystem
{
    EntityQuery query;
    EntityManager entityManager;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        query = SystemAPI.QueryBuilder().WithAll<LifeStateTag>().Build();
        state.RequireForUpdate(query);

        entityManager = state.EntityManager;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

 // Because this OnUpdate accesses managed objects, it cannot be Burst-compiled.
    public void OnUpdate(ref SystemState state)
    {
        var entities = query.ToEntityArray(Allocator.Temp);
        for(int i=0; i< entities.Length;i++)
        {
            var entity = entities[i];
            
            var level = entityManager.GetComponentData<LifeStateTag>(entity).level;

          
        }
        entities.Dispose();
    }
}