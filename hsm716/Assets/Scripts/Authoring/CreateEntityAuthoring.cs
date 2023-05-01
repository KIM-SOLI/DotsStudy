using Unity.Entities;

// Authoring MonoBehaviours are regular GameObject components.
// They constitute the inputs for the baking systems which generates ECS data.
class CreateEntityAuthoring : UnityEngine.MonoBehaviour
{
    class BuildObjectBaker : Baker<CreateEntityAuthoring>
    {
        public override void Bake(CreateEntityAuthoring authoring)
        {
            AddComponent(new BuildObject
            {
            });
        }
    }
}

// An ECS component.
// An empty component is called a "tag component".
struct BuildObject : IComponentData
{
}