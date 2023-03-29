using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.ComponentsAndTags;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct ZombieWalkAspect : IAspect
{
    public readonly Entity Entity;

    private readonly TransformAspect _transform;
    private readonly RefRW<ZombieTimer> _walkTimer;
    private readonly RefRO<ZombieWalkProperties> _walkProperties;
    private readonly RefRW<ZombieHeading> _heading;

    private float WalkSpeed => _walkProperties.ValueRO.WalkSpeed;
    private float WalkAmplitude => _walkProperties.ValueRO.WalkAmplitude;
    private float WalkFrequency => _walkProperties.ValueRO.WalkFrequency;
    private float Heading
    {
        get => _heading.ValueRO.Value;
        set => _heading.ValueRW.Value = value;
    }

    private float WalkTimer
    {
        get => _walkTimer.ValueRO.Value;
        set => _walkTimer.ValueRW.Value = value;
    }

    public void ChangeHeading(float3 targetPos)
    {
        var zombieHeading = MathHelpers.GetHeading(_transform.Position, targetPos);
        Heading = zombieHeading;
    }

    public void Walk(float deltaTime)
    {
        //float3 pos = _transform.LocalPosition;

        //if (Vector3.Distance(targetPos, pos) > 3f)
        //{
        //    //float3 moveDirection = math.normalize(targetPos - pos);

        //    //_transform.LocalPosition += moveDirection * deltaTime * WalkSpeed;
        //    //_transform.LocalPosition = new float3(_transform.LocalPosition.x, 0.01f, _transform.LocalPosition.z);

        //    _transform.Position += _transform.Forward * WalkSpeed * deltaTime;
        //}

        WalkTimer += deltaTime;
        _transform.Position += _transform.Forward * WalkSpeed * deltaTime;

        var swayAngle = WalkAmplitude * math.sin(WalkFrequency * WalkTimer);
        _transform.Rotation = quaternion.Euler(0, Heading, swayAngle);
    }

    public bool IsInStoppingRange(float3 targetPos, float brainRadiusSq)
    {
        Debug.Log("distancesq : " + math.distancesq(targetPos, _transform.Position));
        return math.distancesq(targetPos, _transform.Position) <= brainRadiusSq;
    }
}
