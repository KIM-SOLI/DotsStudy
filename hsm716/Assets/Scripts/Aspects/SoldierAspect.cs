﻿using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.UIElements.Experimental;

public readonly partial struct SoldierAspect : IAspect
{
    public readonly Entity Entity;
    public readonly RefRO<Soldier> m_Soldier;
    readonly RefRO<MoveToTarget> targetComponent;
    readonly RefRO<MySoldierTag> enemyTagComponent;
    private readonly TransformAspect transform;


    public void Move(float deltaTime) 
    {

        // Notice that this is a lambda being passed as parameter to ForEach.
        float3 targetPosition = targetComponent.ValueRO.targetPosition;
        float3 pos = transform.LocalPosition;
        float3 moveDirection = math.normalize(targetPosition - pos);
        
        if (Vector3.Distance(targetPosition, pos) > 1f)
        {
            

            transform.LocalPosition += moveDirection * deltaTime * targetComponent.ValueRO.moveSpeed;
            transform.LocalPosition = new float3(transform.LocalPosition.x, 0.01f, transform.LocalPosition.z);
            
        }
        transform.LookAt(targetPosition);
    }

    public bool IsInTargetRange(float3 targetPosition, float range)
    {
        return math.distancesq(targetPosition, transform.LocalTransform.Position) <= range - 1;
    }


}