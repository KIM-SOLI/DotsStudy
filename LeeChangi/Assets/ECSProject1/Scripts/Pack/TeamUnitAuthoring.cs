using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;


public class TeamUnitAuthoring : UnityEngine.MonoBehaviour
{
    public class TeamUnitBaker : Baker<TeamUnitAuthoring>
    {
        public override void Bake(TeamUnitAuthoring authoring)
        {
            AddComponent(new TeamUnitComponentData { });
            AddComponent(new TeamUnitTarget { });
        }
    }
}


struct TeamUnitComponentData : IComponentData
{
	public int TeamIndex;
}

struct TeamUnitTarget : IComponentData
{
    public TeamUnitComponentData target;
}


readonly partial struct TeamUnitAspect : IAspect
{
    readonly RefRO<TeamUnitComponentData> _unit;
    readonly RefRW<TeamUnitTarget> _target;
}