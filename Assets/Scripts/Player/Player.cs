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

        inputActions.player.Move.performed += OnMove;
        inputActions.player.Move.canceled += OnMove;

        inputActions.player.Upgrade.performed += OnUpgradePerformed;
        inputActions.player.Upgrade.canceled += OnUpgradeCanceled;

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
        inputActions.Disable();
    }

    /// <summary>
    /// Handle upgrade hold progress and trigger upgrade UI.
    /// </summary>
    private void Update()
    {
        if (!isHoldingUpgrade) return;

        upgradeHoldTimer += Time.unscaledDeltaTime;
        UIManager.Instance.SetUpgradeHoldProgress(upgradeHoldTimer / upgradeHoldDuration);

        if (upgradeHoldTimer >= upgradeHoldDuration)
        {
            TriggerUpgradeScreen();
            OnUpgradeCanceled(default);
        }
    }

    /// <summary>
    /// Process physics-based movement and rotation each fixed frame.
    /// </summary>
    private void FixedUpdate()
    {
        if (!GameManager.Instance.GetCanPlayerMove()) return;
        if (TimeManager.Instance.IsPaused) return;

        HandleMovement();
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
        if (!ProgressManager.Instance.IsUpgradeAvailable) return;

        isHoldingUpgrade = true;
        upgradeHoldTimer = 0f;
    }

    private void OnUpgradeCanceled(InputAction.CallbackContext ctx)
    {
        isHoldingUpgrade = false;
        upgradeHoldTimer = 0f;
        UIManager.Instance.SetUpgradeHoldProgress(0f);
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
    #endregion

    #region Upgrade
    private void TriggerUpgradeScreen()
    {
        UIManager.Instance.ShowUpgradeMenu();
    }
    #endregion
}
