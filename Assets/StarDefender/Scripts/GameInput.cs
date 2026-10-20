using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Central input using the new Input System.
/// Exposes C# events for Move and Fire so other scripts subscribe without polling.
/// </summary>
public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    public event Action<float> OnMove;
    public event Action OnFirePressed;

    InputSystem_Actions controls;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        controls = new InputSystem_Actions();
    }

    void OnEnable()
    {
        if (controls == null) return;
        controls.Player.Enable();
        controls.Player.Attack.performed += HandleFire;
    }

    void OnDisable()
    {
        if (controls == null) return;
        controls.Player.Attack.performed -= HandleFire;
        controls.Player.Disable();
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        controls?.Dispose();
        controls = null;
    }

    void Update()
    {
        if (controls == null) return;
        float horizontal = controls.Player.Move.ReadValue<Vector2>().x;
        OnMove?.Invoke(horizontal);
    }

    void HandleFire(InputAction.CallbackContext ctx)
    {
        OnFirePressed?.Invoke();
    }
}
