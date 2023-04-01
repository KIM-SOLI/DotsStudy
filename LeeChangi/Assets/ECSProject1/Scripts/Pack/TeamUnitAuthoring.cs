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
        }
    }
}


struct TeamUnitComponentData : IComponentData
{
	public int TeamIndex;
}

struct RangedWeaponComponentData : IComponentData
{
    public float Range;
}

struct EnemyTargetComponentData : IEnableableComponent
{
    public Entity target;
}

readonly partial struct TeamUnitAspect : IAspect
{
    readonly RefRO<TeamUnitComponentData> _unit;
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

    public int TeamIndex
    {
        get => _unit.ValueRO.TeamIndex;
    }

    public float3 WorldPosition
    {
        get => Transform.LocalPosition;
        set => Transform.LocalPosition = value;
    }
    
}