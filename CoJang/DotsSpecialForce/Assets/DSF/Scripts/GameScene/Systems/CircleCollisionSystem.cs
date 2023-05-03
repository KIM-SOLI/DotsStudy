using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateAfter(typeof(TransformSystemGroup))]
public partial struct CircleCollisionSystem : ISystem
{
    EntityQuery chaserQuery;
    EntityQuery bulletQuery;
    EntityTypeHandle typeHandle;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var chaserQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<ChaserTag>();
        chaserQuery = state.GetEntityQuery(chaserQueryBuilder);
        state.RequireForUpdate(chaserQuery);
        chaserQueryBuilder.Dispose();

        var bulletQueryBuilder = new EntityQueryBuilder(Allocator.TempJob).WithAll<BulletTag>();
        bulletQuery = state.GetEntityQuery(bulletQueryBuilder);
        state.RequireForUpdate(bulletQuery);
        bulletQueryBuilder.Dispose();

        typeHandle = state.GetEntityTypeHandle();
    }

    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        typeHandle.Update(ref state);

        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var chaserArray = chaserQuery.ToEntityArray(Allocator.TempJob);
        var bulletArray = bulletQuery.ToEntityArray(Allocator.TempJob);

        var job = new CircleCollisionJob
        {
            boundary = 1.5f,
            localTransformType = state.GetComponentTypeHandle<LocalTransform>(true),
            chaserComponentType = state.GetComponentTypeHandle<ChaserComponent>(true),
            bulletTagType = state.GetComponentTypeHandle<BulletTag>(true),

            ecb = ecb.AsParallelWriter(),
            entityHandle = typeHandle,

            chaserEntites = chaserArray,
            bulletEntites = bulletArray,

            otherTransforms = state.GetComponentLookup<LocalTransform>(true),
            bullets = state.GetComponentLookup<BulletTag>(true),
            destroyTags = state.GetComponentLookup<DestoryTag>(true),
        };

        state.Dependency = job.ScheduleParallel(chaserQuery, state.Dependency);

        chaserArray.Dispose(state.Dependency);
        bulletArray.Dispose(state.Dependency);
    }
}

public partial struct CircleCollisionJob : IJobChunk
{
    [ReadOnly] public float boundary;
    [ReadOnly] public float deltaTime;

    [ReadOnly] public ComponentTypeHandle<LocalTransform> localTransformType;
    [ReadOnly] public ComponentTypeHandle<ChaserComponent> chaserComponentType;
    [ReadOnly] public ComponentTypeHandle<BulletTag> bulletTagType;

    [ReadOnly] public ComponentLookup<LocalTransform> otherTransforms;
    [ReadOnly] public ComponentLookup<BulletTag> bullets;
    [ReadOnly] public ComponentLookup<DestoryTag> destroyTags;

    [ReadOnly] public NativeArray<Entity> chaserEntites;
    [ReadOnly] public NativeArray<Entity> bulletEntites;

    [ReadOnly] public EntityTypeHandle entityHandle;
    public EntityCommandBuffer.ParallelWriter ecb;

    public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
    {
        var entities = chunk.GetNativeArray(entityHandle);
        var transforms = chunk.GetNativeArray(ref localTransformType);
        var chaserDatas = chunk.GetNativeArray(ref chaserComponentType);

        for (int i = 0; i < entities.Length; i++) // Chunk 내 Entity들
        {
            var entity = entities[i];
            var destroyTag = destroyTags[entity];

            if (destroyTag.IsDestoryed)
                continue;

            var transform = transforms[i];
            var chaserData = chaserDatas[i];

            for (int j = 0; j < chaserEntites.Length; j++) // 다른 모든(Query 내의) Entity들
            {
                var otherEntity = chaserEntites[j];
                var otherDestroyTag = destroyTags[otherEntity];

                if (entity == otherEntity) // 본인을 제외
                    continue;

                if (otherDestroyTag.IsDestoryed)
                    continue;

                var position = otherTransforms[otherEntity].Position;

                float distance = math.distancesq(transform.Position, position);
                if (distance <= boundary)
                {
                    float3 pushDirection = math.normalize(transform.Position - position);
                    float pushDistance = (boundary - distance) * 0.5f;

                    ecb.SetComponent(unfilteredChunkIndex, entity, new LocalTransform
                    {
                        Position = transform.Position + pushDirection * pushDistance,
                        Rotation = transform.Rotation,
                        Scale = transform.Scale,
                    });
                }
            }

            for (int j = 0; j < bulletEntites.Length; j++) // 총알과 충돌 처리
            {
                var bullet = bulletEntites[j];
                var bulletData = bullets[bullet];
                var bulletDestroyTag = destroyTags[bullet];

                if (bulletData.self == Entity.Null || bulletDestroyTag.IsDestoryed)
                {
                    continue;
                }

                if (destroyTag.IsDestoryed)
                {
                    break;
                }

                var position = otherTransforms[bullet].Position;

                float distance = math.distancesq(transform.Position, position);
                if (distance < boundary)
                {
                    // HP 감소 처리
                    chaserData.HP -= bulletData.BulletDamage;

                    // Death 처리
                    if (chaserData.HP <= 0)
                    {
                        //Debug.Log("chaser Destoyed!");
                        destroyTag.IsDestoryed = true;
                    }

                    ecb.SetComponent(unfilteredChunkIndex, entity, chaserData);
                    ecb.SetComponent(unfilteredChunkIndex, entity, destroyTag);

                    // 관통력 처리
                    bulletData.PanetrateNum--;
                    ecb.SetComponent(unfilteredChunkIndex, bullet, bulletData);
                }
            }
        }
    }
}
