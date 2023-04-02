using Unity.Entities;

class ConfigAuthoring : UnityEngine.MonoBehaviour
{
    public UnityEngine.GameObject TankPrefab;
    public UnityEngine.GameObject MeSoldierPrefab;
    public int TankCount;
    public float SafeZoneRadius;

    class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            AddComponent(new Config
            {
                TankPrefab = GetEntity(authoring.TankPrefab),
                MeSoldierPrefab = GetEntity(authoring.MeSoldierPrefab),
                TankCount = authoring.TankCount,
                SafeZoneRadius = authoring.SafeZoneRadius
            });
        }
    }
}

struct Config : IComponentData
{
    public Entity TankPrefab;
    public Entity MeSoldierPrefab;
    public int TankCount;
    public float SafeZoneRadius;
}