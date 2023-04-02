using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.UIElements.Experimental;

public readonly partial struct SoldierAspect : IAspect
{
    public readonly Entity Entity;
    public readonly RefRO<Soldier> m_Soldier;
    readonly RefRO<MoveToTarget> targetComponent;
    
    private readonly TransformAspect transform;


    public void Move(float deltaTime) 
    {
        // Notice that this is a lambda being passed as parameter to ForEach.
        float3 targetPosition = targetComponent.ValueRO.targetPosition;
        float3 pos = transform.LocalPosition;
        
        if (Vector3.Distance(targetPosition, pos) > 1f)
        {
            float3 moveDirection = math.normalize(targetPosition - pos);

            transform.LocalPosition += moveDirection * deltaTime * targetComponent.ValueRO.moveSpeed;
            transform.LocalPosition = new float3(transform.LocalPosition.x, 0.01f, transform.LocalPosition.z);
            transform.LookAt(targetPosition);
        }
    }

    public void EnemyMove(float deltaTime)
    {
        float3 targetPosition = targetComponent.ValueRO.targetPosition;
        float3 pos = transform.LocalPosition;

        float3 moveDirection = math.normalize(targetPosition - pos);

        transform.LocalPosition += targetPosition * deltaTime * targetComponent.ValueRO.moveSpeed;
        transform.LocalPosition = new float3(transform.LocalPosition.x, 0.01f, transform.LocalPosition.z);
        transform.LookAt(targetPosition);
    }


}