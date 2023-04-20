using Unity.Entities;

public class EnemyAuthoring : UnityEngine.MonoBehaviour
{
    public float Speed = 1f;
    public float HP = 100f;

    public class EnemyBaker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring authoring)
        {
            AddComponent<EnemyTag>();
            AddComponent(new DefaltCharacterComponent
            {
                Speed = authoring.Speed,
                HP = authoring.HP,
            });
        }
    }
}

public struct EnemyTag : IComponentData
{
}
