using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Runtime wrapper for Move (Vector2) and Attack/Fire (Button).
/// Matches the tutorial InputSystem_Actions API used in Prototype 4.
/// </summary>
public class InputSystem_Actions : IDisposable
{
    public InputActionAsset asset { get; }
    readonly PlayerActions m_Player;

    public InputSystem_Actions()
    {
        asset = ScriptableObject.CreateInstance<InputActionAsset>();
        var map = asset.AddActionMap("Player");

        var move = map.AddAction("Move", type: InputActionType.Value, expectedControlLayout: "Vector2");
        move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        move.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        move.AddBinding("<Gamepad>/leftStick");

        var attack = map.AddAction("Attack", type: InputActionType.Button);
        attack.AddBinding("<Keyboard>/space");
        attack.AddBinding("<Mouse>/leftButton");
        attack.AddBinding("<Gamepad>/buttonSouth");

        m_Player = new PlayerActions(map);
    }

    public PlayerActions Player => m_Player;

    public void Dispose()
    {
        asset?.Disable();
        if (asset != null)
        {
            UnityEngine.Object.Destroy(asset);
        }
    }

    public struct PlayerActions
    {
        readonly InputActionMap m_Map;

        public PlayerActions(InputActionMap map)
        {
            m_Map = map;
        }

        public InputAction Move => m_Map.FindAction("Move");
        public InputAction Attack => m_Map.FindAction("Attack");

        public void Enable() => m_Map.Enable();
        public void Disable() => m_Map.Disable();
    }
}
