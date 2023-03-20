using Assets.Scripts.AuthoringAndMono;
using Assets.Scripts.ComponentsAndTags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnTombstoneSystem: ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GraveyardProperties>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;
        var graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardProperties>();
        var graveyard = SystemAPI.GetAspectRW<GraveyardAspect>(graveyardEntity);

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var tombstoneOffset = new float3(0f, -2f, 1f);

        for (var i = 0; i < graveyard.NumberTombstonesToSpawn; i++)
        {
            var newTombstone = ecb.Instantiate(graveyard.TombstonePrefab);
            var newTombstoneTransform = graveyard.GetRandomTombstoneTransform();
            ecb.SetComponent(newTombstone, new LocalToWorldTransform { Value = newTombstoneTransform });

            var newZombieSpawnPoint = newTombstoneTransform.Position + tombstoneOffset;

            graveyard.ZombieSpawnPoints.Add(new ZombieSpawnPoints { Value = newZombieSpawnPoint });
        }

        ecb.Playback(state.EntityManager);
    }
}
