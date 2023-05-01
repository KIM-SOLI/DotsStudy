using Unity.Entities;
using UnityEngine;

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
               // QuadMesh = GetEntity(authoring.quadMesh, TransformUsageFlags.Default),
               // SpriteSheetMaterial = GetEntity(authoring.spriteSheetMaterial),

                TankCount = authoring.TankCount,
                SafeZoneRadius = authoring.SafeZoneRadius,
            });
        }
    }
}

struct Config : IComponentData
{
    public Entity TankPrefab;
    public Entity MeSoldierPrefab;
    //public Entity QuadMesh;
    //public Entity SpriteSheetMaterial;
    public int TankCount;
    public float SafeZoneRadius;
}