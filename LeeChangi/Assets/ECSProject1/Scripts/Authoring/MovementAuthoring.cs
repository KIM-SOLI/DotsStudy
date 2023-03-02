using Unity.Entities;

public class MovementAuthoring : UnityEngine.MonoBehaviour
{
    public class MovementBaker : Baker<MovementAuthoring>
    {
        public override void Bake(MovementAuthoring authoring)
        {
            AddComponent(new MovementComponentData { });
        }
    }
}


struct MovementComponentData : IComponentData
{
    
}