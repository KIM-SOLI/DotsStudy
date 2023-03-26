using System;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/*
partial class AnimationSystem : SystemBase
{
    private EntityQuery animationQuery;
    private float deltaTime;

    [BurstCompile]
    protected override void OnCreate()
    {
        animationQuery = GetEntityQuery(typeof(AnimationData), typeof(LocalToWorld));
    }

    protected override void OnUpdate()
    {
        deltaTime = SystemAPI.Time.DeltaTime;

        Entities.WithoutBurst().WithStoreEntityQueryInField(ref animationQuery)
            .ForEach((Entity entity, ref AnimationData animationData, ref LocalToWorld localToWorld) =>
            {
                // �ִϸ��̼� Ŭ������ ���� �ð��� �ش��ϴ� Ű ������ ������ �����´�.
                float3 position;
                quaternion rotationValue;
                //GetKeyframeData(animationData.animationClip, animationData.playbackTime, out position, out rotationValue);

                position = float3.zero;
                rotationValue = quaternion.identity;

                // �ִϸ��̼� Ŭ������ �ð��� �ش��ϴ� Ű ������ ������ �����´�.
                foreach (var binding in AnimationUtility.GetCurveBindings(animationData.animationClip))
                {
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(animationData.animationClip, binding);
                    if (curve != null)
                    {
                        if (binding.propertyName == "m_LocalPosition.x")
                        {
                            position.x = curve.Evaluate(animationData.playbackTime);
                        }
                        else if (binding.propertyName == "m_LocalPosition.y")
                        {
                            position.y = curve.Evaluate(animationData.playbackTime);
                        }
                        else if (binding.propertyName == "m_LocalPosition.z")
                        {
                            position.z = curve.Evaluate(animationData.playbackTime);
                        }
                        else if (binding.propertyName == "m_LocalRotation.x")
                        {
                            rotationValue.value.x = curve.Evaluate(animationData.playbackTime);
                        }
                        else if (binding.propertyName == "m_LocalRotation.y")
                        {
                            rotationValue.value.y = curve.Evaluate(animationData.playbackTime);
                        }
                        else if (binding.propertyName == "m_LocalRotation.z")
                        {
                            rotationValue.value.z = curve.Evaluate(animationData.playbackTime);
                        }
                        else if (binding.propertyName == "m_LocalRotation.w")
                        {
                            rotationValue.value.w = curve.Evaluate(animationData.playbackTime);
                        }
                    }
                }


                // ��ƼƼ�� ��ȯ ����� �ִϸ��̼ǿ� �°� ������Ʈ�Ѵ�.
                var rotation = math.mul(animationData.startRotation, rotationValue);
                var translation = position + animationData.startPosition;
                var scale = new float3(1f);
                var localToWorldMatrix = new float4x4(rotation, translation) * float4x4.Scale(scale);
                localToWorld.Value = localToWorldMatrix;

                // �ִϸ��̼� ��� �ð��� ������Ʈ�Ѵ�.
                animationData.playbackTime += deltaTime;
                if (animationData.playbackTime >= animationData.animationClip.length)
                {
                    animationData.playbackTime = 0f;
                }
            }).Run();

    }

   
    private void GetKeyframeData(AnimationClip animationClip, float time, out float3 position, out quaternion rotation)
    {
        position = float3.zero;
        rotation = quaternion.identity;

        // �ִϸ��̼� Ŭ������ �ð��� �ش��ϴ� Ű ������ ������ �����´�.
        foreach (var binding in AnimationUtility.GetCurveBindings(animationClip))
        {
            var curve = AnimationUtility.GetEditorCurve(animationClip, binding);
            if (curve != null)
            {
                if (binding.propertyName == "m_LocalPosition.x")
                {
                    position.x = curve.Evaluate(time);
                }
                else if (binding.propertyName == "m_LocalPosition.y")
                {
                    position.y = curve.Evaluate(time);
                }
                else if (binding.propertyName == "m_LocalPosition.z")
                {
                    position.z = curve.Evaluate(time);
                }
                else if (binding.propertyName == "m_LocalRotation.x")
                {
                    rotation.value.x = curve.Evaluate(time);
                }
                else if (binding.propertyName == "m_LocalRotation.y")
                {
                    rotation.value.y = curve.Evaluate(time);
                }
                else if (binding.propertyName == "m_LocalRotation.z")
                {
                    rotation.value.z = curve.Evaluate(time);
                }
                else if (binding.propertyName == "m_LocalRotation.w")
                {
                    rotation.value.w = curve.Evaluate(time);
                }
            }
        }
    }
}
*/