using Unity.Entities;
using UnityEngine;

public class SpawnAuthoring : UnityEngine.MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRate;

    public class SpawnBaker : Baker<SpawnAuthoring>
    {
        public override void Bake(SpawnAuthoring authoring)
        {
            AddComponent(new Spawn
            {
                enemyPrefab = GetEntity(authoring.enemyPrefab),
                spawnRate = authoring.spawnRate,
            });
        }
    }
}

public struct Spawn : IComponentData
{
    public Entity enemyPrefab;
    public float spawnRate;
}
