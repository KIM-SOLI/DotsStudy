using Unity.Entities;
using Unity.Transforms;

public class MovementAuthoring : UnityEngine.MonoBehaviour
{
    public class MovementBaker : Baker<MovementAuthoring>
    {
        public override void Bake(MovementAuthoring authoring)
        {
            AddComponent(new MovementComponentData
            {

            });
        }
    }
}

struct MovementComponentData : IComponentData
{

}

readonly partial struct MovementAspect : IAspect
{
    readonly RefRO<MovementComponentData> compData;
    readonly TransformAspect transform;

    public readonly Entity self;

    public UnityEngine.Vector3 WorldPosition
    {
        get => transform.LocalPosition;
        set => transform.LocalPosition = value;
    }
}