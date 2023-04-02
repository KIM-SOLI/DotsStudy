using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;


namespace Sample1
{

    public class TeamUnitAuthoring : UnityEngine.MonoBehaviour
    {
        public class TeamUnitBaker : Baker<TeamUnitAuthoring>
        {
            public override void Bake(TeamUnitAuthoring authoring)
            {
                AddComponent(new TeamUnitComponentData { });
                AddComponent(new EnemyTargetComponentData { });
                AddComponent(new RangedWeaponComponentData { });   
            }
        }
    }


    public struct TeamUnitComponentData : IComponentData
    {
        public int TeamIndex;
    }

    public struct RangedWeaponComponentData : IComponentData
    {
        public float Range;
    }

    public struct EnemyTargetComponentData : IComponentData, IEnableableComponent
    {
        public Entity target;
    }

    public readonly partial struct TeamUnitAspect : IAspect
    {
        readonly RefRW<TeamUnitComponentData> _unit;
        readonly TransformAspect Transform;

        public readonly Entity Self;

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

}
