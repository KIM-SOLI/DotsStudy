using Unity.Entities;
using Unity.Mathematics;

public class BulletAuthoring : UnityEngine.MonoBehaviour
{
    public float3 Speed = 1f;

    public class BulletBaker : Baker<BulletAuthoring>
    {
        public override void Bake(BulletAuthoring authoring)
        {
            AddComponent(new Bullet
            {
                Speed = authoring.Speed,
            });
        }
    }
}

public struct Bullet : IComponentData
{
    public float3 Speed;
}
