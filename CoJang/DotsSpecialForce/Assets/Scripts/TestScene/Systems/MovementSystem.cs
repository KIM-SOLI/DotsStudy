using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.InputSystem;

[UpdateAfter(typeof(InputSystem))]
[BurstCompile]
partial struct MovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        Debug.Log("MovementSystem OnCreate!");

        InputSystem.inputActionMap.FindAction("Click").performed += OnMouseClick;
        InputSystem.inputActionMap.FindAction("MouseMove").performed += OnMouseMove;
        InputSystem.inputActionMap.FindAction("Movement").performed += OnMovement;
    }

    public void OnDestroy(ref SystemState state)
    {
        Debug.Log("MovementSystem OnDestroy!");
    }

    public void OnUpdate(ref SystemState state)
    {

    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        Debug.Log("Clicked!");
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        Debug.Log("Mouse Move!" + context.ReadValue<Vector2>());
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        var movementAction = InputSystem.inputActionMap["Movement"];

        if (InputSystem.IsKeyDown(movementAction, "W"))
        {
            Debug.Log("MovementSystem w");
        }
        if (InputSystem.IsKeyDown(movementAction, "A"))
        {
            Debug.Log("MovementSystem a");
        }
        if (InputSystem.IsKeyDown(movementAction, "S"))
        {
            Debug.Log("MovementSystem s");
        }
        if (InputSystem.IsKeyDown(movementAction, "D"))
        {
            Debug.Log("MovementSystem d");
        }
    }
}
