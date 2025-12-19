using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Player Movement settings")]
    [SerializeField] private GameObject playerVisual;
    private Rigidbody rb;

    private PlayerInputActions inputActions;

    private float movementSpeed;
    private float acceleration;
    private float deceleration;
    private Vector3 currentVelocity = Vector3.zero;

    private float dashSpeed;
    private float dashDuation;
    private float dashCooldown;
    private bool isDashing = false;
    private bool canDash = true;
    private Vector3 dashDirection = Vector3.zero;

    private PlayerSettings playerSettings;

    private PlayerWeapon playerWeapon;
    private bool isFireHolding;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        playerSettings = GetComponent<PlayerSettings>();
        playerWeapon = GetComponent<PlayerWeapon>();
        rb = GetComponent<Rigidbody>();

        if (playerSettings != null)
        {
            movementSpeed = playerSettings.DefaultMovementSpeed;
            acceleration = playerSettings.DefaultAcceleration;
            deceleration = playerSettings.DefaultDeceleration;
            dashSpeed = playerSettings.DefaultDashSpeed;
            dashDuation = playerSettings.DefaultDashDuration;
            dashCooldown = playerSettings.DefaultDashCooldown;
        }
    }

    private void Start()
    {
        inputActions.player.Dash.performed += ctx => HandleDash();
        
    }

    private void Update()
    {
        //* DEBUG *//
        if(Input.GetKeyDown(KeyCode.P))
        {
            // Get the new player settings values
            if (playerSettings != null)
            {
                movementSpeed = playerSettings.DefaultMovementSpeed;
                acceleration = playerSettings.DefaultAcceleration;
                deceleration = playerSettings.DefaultDeceleration;
                dashSpeed = playerSettings.DefaultDashSpeed;
                dashDuation = playerSettings.DefaultDashDuration;
                dashCooldown = playerSettings.DefaultDashCooldown;
            }
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            // Dash movement handled on physics step for smoothness
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
        }
        else
        {
            TryMovement();
        }

        TryRotate();
    }

    private void TryMovement()
    {
        Vector2 moveInput = inputActions.player.Move.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 targetVelocity = moveDir.normalized * movementSpeed;

        currentVelocity = Vector3.SmoothDamp
        (
            currentVelocity,
            targetVelocity,
            ref currentVelocity, // reuse as velocity ref is fine; or use a separate ref var
            (moveDir.sqrMagnitude > 0.001f) ? (1f / acceleration) : (1f / deceleration),
            Mathf.Infinity,
            Time.fixedDeltaTime
        );

        rb.MovePosition(rb.position + currentVelocity * Time.fixedDeltaTime);
    }

    private void TryRotate()
    {
        // Mouse Rotation
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            Vector3 tagetPoint = new Vector3(hitInfo.point.x, playerVisual.transform.position.y, hitInfo.point.z);
            Vector3 direction = (tagetPoint - playerVisual.transform.position).normalized;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                playerVisual.transform.rotation = Quaternion.Slerp(playerVisual.transform.rotation, targetRotation, 0.2f);
            }
        }

        // Joystick Rotation
        Vector2 rotateInput = inputActions.player.Rotate.ReadValue<Vector2>();
        Vector3 joystickDir = new Vector3(rotateInput.x , 0f, rotateInput.y);

        if (joystickDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(joystickDir, Vector3.up);
            playerVisual.transform.rotation = Quaternion.Slerp(playerVisual.transform.rotation, targetRotation, 0.2f);
        }
    }

    private void HandleDash()
    {
        Debug.Log("Dash");
        if (!canDash || isDashing) return;

        // Decide the direction to dash in.
        Vector2 moveInput = inputActions.player.Move.ReadValue<Vector2>();
        Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y);

        if (dir.sqrMagnitude < 0.01f)
        {
            // fallback -> dash in facing direciotn
            dir = playerVisual.transform.forward;
        }
        dashDirection = dir.normalized;

        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        canDash = false;

        rb.velocity = Vector3.zero;

        float elapsed = 0f;
        while(elapsed < dashDuation)
        {
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
        dashDirection = Vector3.zero;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Start cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;

    }
    private void OnEnable()
    {
        inputActions.player.Shoot.performed += ctx => isFireHolding = true;
        inputActions.player.Shoot.canceled += ctx => isFireHolding = false;
        inputActions.player.Reload.performed += ctx => playerWeapon.StartReload();

        inputActions.Enable();   
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    public bool IsFireHolding => isFireHolding;
   
}

