using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(SimulationSystemGroup))]
public partial struct DestroySystem : ISystem
{
    //EntityQuery entityQuery;
    public ComponentLookup<DestoryTag> tagComponentLookup;

    public void OnCreate(ref SystemState state)
    {
        //var entityQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<DestoryTag>();
        //entityQuery = state.GetEntityQuery(entityQueryBuilder);
        //state.RequireForUpdate(entityQuery);
        //entityQueryBuilder.Dispose();

        tagComponentLookup = state.GetComponentLookup<DestoryTag>(true);
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        tagComponentLookup.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var job = new DestroyJob
        {
            ecb = ecb.AsParallelWriter(),
            tags = tagComponentLookup,
        };
        job.ScheduleParallel();
    }
}

public partial struct DestroyJob : IJobEntity
{
    [ReadOnly] public ComponentLookup<DestoryTag> tags;

    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute(in DestoryTag tag)
    {
        if (tag.IsDestoryed && tag.self != Entity.Null)
        {
            //Debug.Log("Entity Destroy Completed");
            ecb.DestroyEntity(tag.self.Index, tag.self);
        }
    }
}
