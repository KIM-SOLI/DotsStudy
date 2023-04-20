using Unity.Entities;

public class PlayerAuthoring : UnityEngine.MonoBehaviour
{
    public float Speed = 3f;
    public float HP = 300f;

    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            AddComponent<PlayerTag>();
            AddComponent(new DefaltCharacterComponent
            {
                Speed = authoring.Speed,
                HP = authoring.HP,
            });
        }
    }

}

public struct PlayerTag : IComponentData
{
}