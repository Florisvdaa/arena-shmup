using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player movement, rotation, upgrade input, and interaction with game state.
/// </summary>
public class Player : MonoBehaviour
{ 
    #region Inspector Fields
    [Header("Movement Settings")]
    [Tooltip("Duration required to hold the upgrade input before opening the upgrade UI.")]
    [SerializeField] private float upgradeHoldDuration = 1.5f;
    #endregion

    #region Private Fields
    private Vector2 moveInput;
    private Rigidbody rb;
    private Transform playerVisual;

    private Camera mainCam;
    private PlayerInputActions inputActions;
    private PlayerSettings playerSettings;

    private bool isHoldingUpgrade = false;
    private float upgradeHoldTimer = 0f;

    private bool isDashing = false;
    private float dashTimer;
    private float dashCooldownTimer = 0f;
    private Vector3 dashDirection;
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize references and subscribe to input events.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerSettings = GetComponent<PlayerSettings>();
        mainCam = Camera.main;

        inputActions = new PlayerInputActions();
        inputActions.Enable();

        SetUpInputs();

        playerVisual = playerSettings.PlayerVisualTransform;
    }

    /// <summary>
    /// Cleanup input subscriptions.
    /// </summary>
    private void OnDestroy()
    {
        inputActions.player.Move.performed -= OnMove;
        inputActions.player.Move.canceled -= OnMove;
        inputActions.player.Upgrade.performed -= OnUpgradePerformed;
        inputActions.player.Upgrade.canceled -= OnUpgradeCanceled;
        inputActions.player.Dash.performed -= OnDashPerformed;
        inputActions.Disable();
    }

    /// <summary>
    /// Handle upgrade hold progress and trigger upgrade UI.
    /// </summary>
    private void Update()
    {
        // Upgrade input handling
        if (isHoldingUpgrade)
        {
            upgradeHoldTimer += Time.unscaledDeltaTime;
            //UIManager.Instance.SetUpgradeHoldProgress(upgradeHoldTimer / upgradeHoldDuration);

            if (upgradeHoldTimer >= upgradeHoldDuration)
            {
                //TriggerUpgradeScreen();
                OnUpgradeCanceled(default);
            }
        }

        // Dash cooldown logic
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
            Debug.Log($"Dash cooldown: {dashCooldownTimer:F2}s remaining");
        }
    }

    /// <summary>
    /// Process physics-based movement and rotation each fixed frame.
    /// </summary>
    private void FixedUpdate()
    {
        if (!GameManager.Instance.GetCanPlayerMove()) return;
        if (TimeManager.Instance.IsPaused) return;

        if (isDashing)
        {
            HandleDash();
        }
        else
        {
            HandleMovement();
        }
        RotateVisualTowardsMouse();
    }
    #endregion

    #region Input Handlers
    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
    private void OnUpgradePerformed(InputAction.CallbackContext ctx)
    {
        //if (!ProgressManager.Instance.IsUpgradeAvailable) return;

        isHoldingUpgrade = true;
        upgradeHoldTimer = 0f;
    }
    private void OnUpgradeCanceled(InputAction.CallbackContext ctx)
    {
        isHoldingUpgrade = false;
        upgradeHoldTimer = 0f;
        //UIManager.Instance.SetUpgradeHoldProgress(0f);
    }
    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        if (isDashing)
        {
            Debug.Log("Tried to dash but already dashing.");
            return;
        }

        if (dashCooldownTimer > 0f)
        {
            Debug.Log($"Tried to dash but on cooldown. Time remaining: {dashCooldownTimer:F2}s");
            return;
        }

        isDashing = true;
        dashTimer = playerSettings.DashDuration;

        // Use look direction
        if (playerVisual != null)
        {
            dashDirection = playerVisual.forward;
        }
        else
        {
            dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        }

        Debug.Log("Dash started!");
    }
    #endregion

    #region Movement & Rotation
    private void HandleMovement()
    {
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        float dt = Time.unscaledDeltaTime;
        rb.MovePosition(rb.position + movement * playerSettings.CurrentMovementSpeed * dt);
    }
    private void RotateVisualTowardsMouse()
    {
        if (playerVisual == null || mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);
            Vector3 dir = hit - playerVisual.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
                playerVisual.rotation = Quaternion.LookRotation(dir);
        }
    }
    private void HandleDash()
    {
        float dt = Time.fixedDeltaTime;
        rb.MovePosition(rb.position + dashDirection * playerSettings.DashSpeed * dt);

        dashTimer -= dt;
        if (dashTimer <= 0f)
        {
            isDashing = false;
            dashCooldownTimer = playerSettings.DashCooldown;
            Debug.Log("Dash ended. Cooldown started.");
        }
    }
    #endregion

    #region Upgrade
    //private void TriggerUpgradeScreen()
    //{
    //    UIManager.Instance.ShowUpgradeMenu();
    //}
    #endregion

    #region Input setup
    private void SetUpInputs()
    {
        inputActions.player.Move.performed += OnMove;
        inputActions.player.Move.canceled += OnMove;

        inputActions.player.Upgrade.performed += OnUpgradePerformed;
        inputActions.player.Upgrade.canceled += OnUpgradeCanceled;

        inputActions.player.Dash.performed += OnDashPerformed;
    }
    #endregion
}

