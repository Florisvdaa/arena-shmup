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

    [Tooltip("Radius of your sphere mesh (world units).")]
    [SerializeField] private float sphereRadius = 0.5f;
    [Tooltip("Time it takes to smooth movement direction (sec). Lower = snappier.")]
    [SerializeField] private float moveSmoothTime = 0.1f;

    [Header("Head Tilt Settings")]
    [Tooltip("Maximum head tilt angle (degrees).")]
    [SerializeField] private float headTiltAngle = 10f;
    #endregion

    #region Private Fields
    private Vector2 moveInput;
    private Vector3 currentMoveDir;
    private Vector3 moveDirVelocity;
    private Rigidbody rb;
    private Transform bodyVisual;
    private Transform headPivot;
    private Transform headVisual;
    private Quaternion headInitialLocalRotation;
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

        //playerVisual = playerSettings.PlayerVisualTransform;
        // fetch visuals from PlayerSettings
        var settings = PlayerSettings.Instance;
        bodyVisual = settings.BodyVisualTransform;
        headPivot = settings.HeadPivotTransform;
        headVisual = settings.HeadVisualTransform;

        headInitialLocalRotation = headPivot != null
            ? headPivot.localRotation
            : Quaternion.identity;
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

        MoveAndRollBody();
        TiltHeadBasedOnMovement();
        RotateHeadVisualTowardsMouse();

        //HandleMovement();
        //RotateVisualTowardsMouse();
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
    //private void HandleMovement()
    //{
    //    Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
    //    float dt = Time.unscaledDeltaTime;
    //    rb.MovePosition(rb.position + movement * playerSettings.CurrentMovementSpeed * dt);
    //}

    //private void RotateVisualTowardsMouse()
    //{
    //    if (playerVisual == null || mainCam == null) return;

    //    Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
    //    Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
    //    if (groundPlane.Raycast(ray, out float enter))
    //    {
    //        Vector3 hit = ray.GetPoint(enter);
    //        Vector3 dir = hit - playerVisual.position;
    //        dir.y = 0f;
    //        if (dir.sqrMagnitude > 0.01f)
    //            playerVisual.rotation = Quaternion.LookRotation(dir);
    //    }
    //}
    #region Movement & Rolling
    private void MoveAndRollBody()
    {
        // 1) compute camera-relative desired dir on XZ
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 desiredDir = Vector3.zero;
        if (input.sqrMagnitude >= 0.01f)
        {
            Vector3 camF = mainCam.transform.forward; camF.y = 0; camF.Normalize();
            Vector3 camR = mainCam.transform.right; camR.y = 0; camR.Normalize();
            desiredDir = (camF * input.z + camR * input.x).normalized;
        }

        // 2) smooth
        currentMoveDir = Vector3.SmoothDamp(
            currentMoveDir,
            desiredDir,
            ref moveDirVelocity,
            moveSmoothTime
        );

        // 3) apply to rb
        float speed = PlayerSettings.Instance.CurrentMovementSpeed;
        Vector3 vel = currentMoveDir * speed;
        vel.y = rb.velocity.y;
        rb.velocity = vel;

        // 4) roll bodyVisual
        if (bodyVisual != null && currentMoveDir.sqrMagnitude > 0.001f)
        {
            float dist = speed * Time.fixedDeltaTime;
            float angleDeg = dist / sphereRadius * Mathf.Rad2Deg;
            Vector3 axis = Vector3.Cross(Vector3.up, currentMoveDir).normalized;
            bodyVisual.Rotate(axis, angleDeg, Space.World);
        }
    }
    #endregion

    #region Head Tilt
    private void TiltHeadBasedOnMovement()
    {
        if (headPivot == null) return;
        Vector3 camF = mainCam.transform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = mainCam.transform.right; camR.y = 0; camR.Normalize();
        float fwd = Vector3.Dot(currentMoveDir, camF);
        float lat = Vector3.Dot(currentMoveDir, camR);
        float pitch = -fwd * headTiltAngle;
        float roll = -lat * headTiltAngle;
        headPivot.localRotation = headInitialLocalRotation
                                 * Quaternion.Euler(pitch, 0f, roll);
    }
    #endregion

    #region Head Look-At
    private void RotateHeadVisualTowardsMouse()
    {
        if (headVisual == null || mainCam == null) return;
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);
            Vector3 dir = hit - headVisual.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
                headVisual.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
    #endregion
    #endregion

    #region Upgrade
    private void TriggerUpgradeScreen()
    {
        UIManager.Instance.ShowUpgradeMenu();
    }
    #endregion
}
