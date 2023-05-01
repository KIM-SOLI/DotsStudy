using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class DestructibleAuthoring : MonoBehaviour
{
    public class DestructibleBaker : Baker<DestructibleAuthoring>
    {
        public override void Bake(DestructibleAuthoring authoring)
        {
            AddComponent(new DestoryTag { IsDestoryed = false });
        }
    }
}

public struct DestoryTag : IComponentData
{
    public bool IsDestoryed;
    public Entity self;
}
