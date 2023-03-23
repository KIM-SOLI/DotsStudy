using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;

class AnimationAuthoring : UnityEngine.MonoBehaviour
{
    class AnimationBaker : Baker<AnimationAuthoring>
    {
        public override void Bake(AnimationAuthoring authoring)
        {
            //AddComponent(new AnimationData());
        }
    }
}
public struct AnimationData : IComponentData
{
    public float playbackTime;
    public float3 startPosition;
    public quaternion startRotation;
}
