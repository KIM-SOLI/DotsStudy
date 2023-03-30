using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

class SoldierAuthoring : UnityEngine.MonoBehaviour
{
    class SoldierBaker : Baker<SoldierAuthoring>
    {
        public override void Bake(SoldierAuthoring authoring)
        {
            

            AddComponent(new MoveToTarget());
            AddComponent(new Soldier()
            {
            });
        }   

    }
}

public struct Soldier : IComponentData
{
}
