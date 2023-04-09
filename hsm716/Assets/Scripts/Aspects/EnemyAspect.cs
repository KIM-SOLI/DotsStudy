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

public readonly partial struct EnemyAspect : IAspect
{
    public readonly Entity Entity;
    public readonly RefRO<Soldier> m_Soldier;
    readonly RefRO<MoveToTarget> targetComponent;
    readonly RefRO<EnemyTag> enemyTagComponent;
    private readonly TransformAspect transform;

    public void EnemyMove(float deltaTime)
    {
        float3 targetPosition = targetComponent.ValueRO.targetPosition;


        transform.LocalPosition += targetPosition * deltaTime * targetComponent.ValueRO.moveSpeed;
        transform.LocalPosition = new float3(transform.LocalPosition.x, 0.01f, transform.LocalPosition.z);
        transform.LookAt( targetPosition);
    }


}