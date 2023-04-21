using Unity.Entities;

public class SpawnAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject enemyPrefab;
    public float spawnRate;
    public int spawnCount;

    public class SpawnBaker : Baker<SpawnAuthoring>
    {
        public override void Bake(SpawnAuthoring authoring)
        {
            AddComponent(new Spawn
            {
                enemyPrefab = GetEntity(authoring.enemyPrefab),
                spawnRate = authoring.spawnRate,
                spawnCount = authoring.spawnCount,
            });
            AddComponent(new EnemyCount { enemyCount = 0 });
        }
    }
}

public struct Spawn : IComponentData
{
    public Entity enemyPrefab;
    public float spawnRate;
    public int spawnCount;
}

public struct EnemyCount : IComponentData
{
    public int enemyCount;
}
