using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Movement settings")]
    [SerializeField] private GameObject playerVisual;
    private Rigidbody rb;

    private PlayerInputActions inputActions;

    private float movementSpeed;
    private float dashSpeed;
    private float dashDuation;

    private PlayerSettings playerSettings;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        playerSettings = GetComponent<PlayerSettings>();
        rb = GetComponent<Rigidbody>();
        
        if (playerSettings != null)
        {
            movementSpeed = playerSettings.DefaultMovementSpeed;
            dashSpeed = playerSettings.DefaultDashSpeed;
            dashDuation = playerSettings.DefaultDashDuration;
        }
    }

    private void Start()
    {
        inputActions.player.Dash.performed += ctx => HandleDash();   
    }

    private void FixedUpdate()
    {
        TryMovement();
    }

    private void TryMovement()
    {
        Vector2 moveInput = inputActions.player.Move.ReadValue<Vector2>();
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.MovePosition(rb.position + move * movementSpeed * Time.fixedDeltaTime);
    }
    private void HandleDash()
    {
        Debug.Log("Dash");
    }

    private void OnEnable()
    {
        inputActions.Enable();   
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
}

