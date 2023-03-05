using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;

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
    public float Speed;
}

struct TeamUnitTarget : IComponentData
{
    public TeamUnitComponentData target;
}


readonly partial struct TeamUnitAspect : IAspect
{
    //readonly RefRO<TeamUnitComponentData> _unit;
    //readonly RefRW<TeamUnitTarget> _target;
    readonly TransformAspect Transform;

    public readonly Entity Self;

    //public TeamUnitComponentData Target
    //{
    //    set
    //    {
    //        _target.ValueRW.target = value;
    //    }
    //}

    public float3 WorldPosition
    {
        get => Transform.LocalPosition;
        set => Transform.LocalPosition = value;
    }

}