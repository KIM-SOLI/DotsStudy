//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Inputs/Actions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @Actions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @Actions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Actions"",
    ""maps"": [
        {
            ""name"": ""PlayerActions"",
            ""id"": ""9d23bd1d-e049-458b-83cb-75fe9f0ae9e8"",
            ""actions"": [
                {
                    ""name"": ""MoveOrSpawn"",
                    ""type"": ""Button"",
                    ""id"": ""ca1c1498-d6cb-4457-a651-3a7c1a1f2b26"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ClickPosition"",
                    ""type"": ""Value"",
                    ""id"": ""a59008a4-3cce-4a2c-ae65-b76523869b5f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""748142e0-538c-44fa-8db6-999947e4ad96"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""MoveOrSpawn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dfffca64-2f7c-447f-9b92-2281c40da38f"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""ClickPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC"",
            ""bindingGroup"": ""PC"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlayerActions
        m_PlayerActions = asset.FindActionMap("PlayerActions", throwIfNotFound: true);
        m_PlayerActions_MoveOrSpawn = m_PlayerActions.FindAction("MoveOrSpawn", throwIfNotFound: true);
        m_PlayerActions_ClickPosition = m_PlayerActions.FindAction("ClickPosition", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerActions
    private readonly InputActionMap m_PlayerActions;
    private IPlayerActionsActions m_PlayerActionsActionsCallbackInterface;
    private readonly InputAction m_PlayerActions_MoveOrSpawn;
    private readonly InputAction m_PlayerActions_ClickPosition;
    public struct PlayerActionsActions
    {
        private @Actions m_Wrapper;
        public PlayerActionsActions(@Actions wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveOrSpawn => m_Wrapper.m_PlayerActions_MoveOrSpawn;
        public InputAction @ClickPosition => m_Wrapper.m_PlayerActions_ClickPosition;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActionsActions instance)
        {
            if (m_Wrapper.m_PlayerActionsActionsCallbackInterface != null)
            {
                @MoveOrSpawn.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnMoveOrSpawn;
                @MoveOrSpawn.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnMoveOrSpawn;
                @MoveOrSpawn.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnMoveOrSpawn;
                @ClickPosition.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnClickPosition;
                @ClickPosition.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnClickPosition;
                @ClickPosition.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnClickPosition;
            }
            m_Wrapper.m_PlayerActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveOrSpawn.started += instance.OnMoveOrSpawn;
                @MoveOrSpawn.performed += instance.OnMoveOrSpawn;
                @MoveOrSpawn.canceled += instance.OnMoveOrSpawn;
                @ClickPosition.started += instance.OnClickPosition;
                @ClickPosition.performed += instance.OnClickPosition;
                @ClickPosition.canceled += instance.OnClickPosition;
            }
        }
    }
    public PlayerActionsActions @PlayerActions => new PlayerActionsActions(this);
    private int m_PCSchemeIndex = -1;
    public InputControlScheme PCScheme
    {
        get
        {
            if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
            return asset.controlSchemes[m_PCSchemeIndex];
        }
    }
    public interface IPlayerActionsActions
    {
        void OnMoveOrSpawn(InputAction.CallbackContext context);
        void OnClickPosition(InputAction.CallbackContext context);
    }
}