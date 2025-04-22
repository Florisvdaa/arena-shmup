using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody rb;

    [Header("Player Health")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    [Header("References")]
    [SerializeField] private Transform playerVisual;

    private Camera mainCam;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCam = Camera.main;

        // Instantiate and enable your input actions
        inputActions = new PlayerInputActions();
        inputActions.Enable();

        // Subscribe to the move input event
        inputActions.player.Move.performed += ctx => OnMove(ctx);
        inputActions.player.Move.canceled += ctx => OnMove(ctx); // Stops movement when input released

        currentHealth = maxHealth;
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
        HandleMovement();
        RotateVisualTowardsMouse();
    }
    private void HandleMovement()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        rb.MovePosition(rb.position + move.normalized * moveSpeed * Time.fixedDeltaTime);
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
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0 )
        {
            // Player is dead
        }
    }
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
