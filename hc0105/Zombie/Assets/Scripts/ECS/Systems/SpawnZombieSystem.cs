using Assets.Scripts.AuthoringAndMono;
using Assets.Scripts.ComponentsAndTags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial class SpawnZombieSystem: SystemBase
{
    private EntityCommandBufferSystem ecbs;

    protected override void OnCreate()
    {
        ecbs = World.GetOrCreateSystemManaged<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = ecbs.CreateCommandBuffer().AsParallelWriter();

        var spawnJob = new SpawnZombieJob
        {
            DeltaTime = deltaTime,
            ECB = ecb
        };

        var spwanJobHandle = spawnJob.Schedule(Dependency);

        var riseJobHandle = new ZombieRiseJob
        {
            DeltaTime = deltaTime,
            ECB = ecb
        }.ScheduleParallel(spwanJobHandle);

        Dependency = JobHandle.CombineDependencies(Dependency, riseJobHandle);

        ecbs.AddJobHandleForProducer(riseJobHandle);
    }

    [BurstCompile]
    public partial struct SpawnZombieJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(GraveyardAspect graveyard, [ChunkIndexInQuery] int chunkIndex)
        {
            graveyard.ZombieSpawnTimer -= DeltaTime;
            if (!graveyard.TimeToSpawnZombie) return;
            if (graveyard.ZombieSpawnPoints.Length == 0) return;

            graveyard.ZombieSpawnTimer = graveyard.ZombieSpawnRate;
            var newZombie = ECB.Instantiate(chunkIndex, graveyard.ZombiePrefab);

            var newZombieTransform = graveyard.GetZombieSpawnPoint();
            ECB.SetComponent(chunkIndex, newZombie, new LocalToWorldTransform { Value = newZombieTransform });

            var zombieHeading = MathHelpers.GetHeading(newZombieTransform.Position, graveyard.Position);
            ECB.SetComponent(chunkIndex, newZombie, new ZombieHeading {Value = zombieHeading});
        }
    }


    [BurstCompile]
    public partial struct ZombieRiseJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;

        [BurstCompile]
        private void Execute(ZombieRiseAspect zombie, [ChunkIndexInQuery] int chunkIndex)
        {
            zombie.Position += math.up() * zombie.RiseRate * DeltaTime;

            if (!zombie.IsAboveGround) return;

            zombie.SetAtGroundLevel();
            ECB.RemoveComponent<ZombieRiseRate>(chunkIndex, zombie.Entity);
            ECB.SetComponentEnabled<ZombieWalkProperties>(chunkIndex, zombie.Entity, true);
        }
    }
}
