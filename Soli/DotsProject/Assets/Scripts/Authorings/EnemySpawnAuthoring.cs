using Unity.Entities;

public class EnemySpawnAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject enemyPrefab;
    public float spawnRate;
    public int spawnCount;

    public class EnemySpawnBaker : Baker<EnemySpawnAuthoring>
    {
        public override void Bake(EnemySpawnAuthoring authoring)
        {
            AddComponent<EnemySpawnTag>();
            AddComponent(new Spawn
            {
                prefab = GetEntity(authoring.enemyPrefab),
                spawnRate = authoring.spawnRate,
                spawnCount = authoring.spawnCount,
            });
            AddComponent(new EnemyCount { enemyCount = 0 });
        }
    }
}

public struct EnemySpawnTag : IComponentData
{

}

public struct Spawn : IComponentData
{
    public Entity prefab;
    public float spawnRate;
    public int spawnCount;
}

public struct EnemyCount : IComponentData
{
    public int enemyCount;
}
