using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;

[UpdateAfter(typeof(PresentationSystemGroup))]
public partial struct DestorySystem : ISystem
{
    EntityQuery entityQuery;

    public void OnCreate(ref SystemState state)
    {
        var entityQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<DestoryTag>();
        entityQuery = state.GetEntityQuery(entityQueryBuilder);
        state.RequireForUpdate(entityQuery);
        entityQueryBuilder.Dispose();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var entityArray = entityQuery.ToEntityArray(Allocator.TempJob);

        var job = new DestoryJob
        {
            ecb = ecb.AsParallelWriter(),
            entites = entityArray,
            tags = state.GetComponentLookup<DestoryTag>(true),
        };
        job.ScheduleParallel();
    }
}

public partial struct DestoryJob : IJobEntity
{
    [ReadOnly] public NativeArray<Entity> entites;
    [ReadOnly] public ComponentLookup<DestoryTag> tags;

    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute(in DestoryTag tag)
    {
        if (tag.IsDestoryed && tag.self != Entity.Null)
        {
            Debug.Log("Entity Destroy Completed");
            ecb.DestroyEntity(tag.self.Index, tag.self);
        }
    }
}
