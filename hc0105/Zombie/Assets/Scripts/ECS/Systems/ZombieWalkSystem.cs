using Assets.Scripts.AuthoringAndMono;
using Assets.Scripts.ComponentsAndTags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    public partial struct ZombieWalkSystem : ISystem
    {
        private EntityManager _entityManager;
        private EntityQuery _characterQuery;
        Entity Target;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _characterQuery = _entityManager.CreateEntityQuery(typeof(CharacterHeading));
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
            var brainScale = SystemAPI.GetComponent<LocalToWorldTransform>(brainEntity).Value.Scale;
            var brainRadius = brainScale * 5f + 0.5f;

            float3 targetPosition = 0f;
            var characters = _characterQuery.ToEntityArray(Allocator.TempJob);
            if (characters.Length > 0)
            {
                Target = characters[0];

                if (Target != Entity.Null)
                {
                    LocalToWorld targetTransform = SystemAPI.GetComponent<LocalToWorld>(Target);
                    targetPosition = targetTransform.Position;
                }
            }

            new ZombieWalkJob
            {
                DeltaTime = deltaTime,
                BrainRadiusSq = brainRadius * brainRadius,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                targetPos = targetPosition

            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ZombieWalkJob : IJobEntity
    {
        public float DeltaTime;
        public float BrainRadiusSq;
        public EntityCommandBuffer.ParallelWriter ECB;
        public float3 targetPos;

        [BurstCompile]
        private void Execute(ZombieWalkAspect zombie, [EntityInQueryIndex] int sortKey)
        {
            zombie.ChangeHeading(targetPos);
            zombie.Walk(DeltaTime);

            //if (zombie.IsInStoppingRange(float3.zero, BrainRadiusSq))
            //if (zombie.IsInStoppingRange(targetPos, 2))
            //{
            //    ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, false);
            //    ECB.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, true);
            //}
        }
    }
}