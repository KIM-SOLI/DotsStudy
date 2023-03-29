using Assets.Scripts.AuthoringAndMono;
using Assets.Scripts.ComponentsAndTags;
using Assets.Scripts.ECS.Authorings;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Assets.Scripts.Systems
{
    [BurstCompile]
    public partial struct CharacterMoveSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
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

            new CharacterMoveJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct CharacterMoveJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(CharacterMoveAspect character, [EntityInQueryIndex] int sortKey)
        {
            //ECB.SetComponentEnabled<CharacterMoveProperties>(sortKey, character.Entity, false);
            character.Walk(DeltaTime);
        }
    }
}