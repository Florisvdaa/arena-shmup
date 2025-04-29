using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    private Vector2 moveInput;
    private Rigidbody rb;

    [Header("References")]
    private Transform playerVisual;

    private Camera mainCam;
    private PlayerInputActions inputActions;
    private PlayerSettings playerSettings;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerSettings = GetComponent<PlayerSettings>();
        mainCam = Camera.main;

        // Instantiate and enable your input actions
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        // Subscribe to the move input event
        inputActions.player.Move.performed += ctx => OnMove(ctx);
        inputActions.player.Move.canceled += ctx => OnMove(ctx); // Stops movement when input released

        playerVisual = playerSettings.PlayerVisualTransform;
    }
    private void OnDestroy()
    {
        // Always clean up event subscriptions!
        inputActions.player.Move.performed -= ctx => OnMove(ctx);
        inputActions.player.Move.canceled -= ctx => OnMove(ctx);
        inputActions.Disable();
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        if (GameManager.Instance.GetCanPlayerMove())
        {
            HandleMovement();
            RotateVisualTowardsMouse();
        }
    }
    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.MovePosition(rb.position + move.normalized * playerSettings.CurrentMovementSpeed * Time.fixedDeltaTime);
        Debug.Log(playerSettings.CurrentMovementSpeed);
    }
    private void RotateVisualTowardsMouse()
    {
        if (playerVisual == null || mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float hitDist))
        {
            Vector3 hitPoint = ray.GetPoint(hitDist);
            Vector3 direction = hitPoint - playerVisual.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                playerVisual.rotation = targetRotation;
            }
        }
    }
}
