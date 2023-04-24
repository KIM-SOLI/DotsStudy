using Unity.Entities;
using UnityEditor.SceneManagement;
using static Unity.Entities.EntitiesJournaling;

public class BulletSpawnAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject bulletPrefab;
    public float spawnRate;
    public int spawnCount;

    public class BulletSpawnBaker : Baker<BulletSpawnAuthoring>
    {
        public override void Bake(BulletSpawnAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<BulletSpawnTag>(entity);
            AddComponent(entity, new Spawn
            {
                prefab = GetEntity(authoring.bulletPrefab, TransformUsageFlags.Dynamic),
                spawnRate = authoring.spawnRate,
                spawnCount = authoring.spawnCount,
            });
        }
    }
}

public struct BulletSpawnTag : IComponentData
{

}