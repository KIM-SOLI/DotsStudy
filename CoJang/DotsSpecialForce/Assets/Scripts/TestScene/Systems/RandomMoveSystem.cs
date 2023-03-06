using Unity.Entities;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
partial struct RandomMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    public void OnUpdate(ref SystemState state)
    {
        var playerTransform = Player.Instance.transform;

        if (Input.GetKeyDown(KeyCode.W))
        {
            playerTransform.position += (playerTransform.forward * 0.5f);
        }
    }
}
