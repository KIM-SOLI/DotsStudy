using Unity.Entities;

public class EnemyAuthoring : UnityEngine.MonoBehaviour
{
    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            AddComponent(new EnemyTag { });
        }
    }
}


struct EnemyTag : IComponentData
{}