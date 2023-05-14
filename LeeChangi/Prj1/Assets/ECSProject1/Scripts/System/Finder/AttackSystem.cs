using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Sample1
{


    public class AttackSystemAuthoring : IGetBakedSystem
    {
        public AttackSystemAuthoring() { }
        public Type GetSystemType()
        {
            return typeof(AttackSystem);
        }
    }

    [DisableAutoCreation]
    [BurstCompile]
    public partial struct AttackSystem : ISystem
    {
        EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            using var entityQueryBuilder = new EntityQueryBuilder(Allocator.Temp)
              .WithAll<LockOnTargetComponentData>()
              .WithAll<BodyStat>();

            entityQuery = state.GetEntityQuery(entityQueryBuilder);
            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton =
            SystemAPI.GetSingleton<
                BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);


            var job = new AttackJob
            {
                Ecb = ecb.AsParallelWriter(),
                deltaTime = SystemAPI.Time.DeltaTime,
            };
            job.ScheduleParallel(entityQuery);
        }
    }



    [BurstCompile]
    public partial struct AttackJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter Ecb;
        [BurstCompile]
        public void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, in BodyStat bodyStat)
        {

            Ecb.SetComponent(chunkIndex, entity, new BodyStat
            {
                Armor = bodyStat.Armor,
                HP = bodyStat.HP - deltaTime * 3,
                hpStateEntity = bodyStat.hpStateEntity,
            });

        }


    }
}