using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRollMovementTest : MonoBehaviour
{
    #region Inspector Fields
    [Header("Visual Transforms")]
    [Tooltip("The sphere mesh that rolls around.")]
    [SerializeField] private Transform bodyVisual;
    [Tooltip("The head/dome pivot that tilts based on movement.")]
    [SerializeField] private Transform headPivot;
    [Tooltip("The head mesh that rotates to look at the cursor.")]
    [SerializeField] private Transform headVisual;

    [Header("Movement Settings")]
    [Tooltip("Ground speed (units/sec).")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("Radius of your sphere mesh (in world units).")]
    [SerializeField] private float sphereRadius = 0.5f;
    [Tooltip("Time it takes to smooth movement direction (sec). Lower = snappier.")]
    [SerializeField] private float moveSmoothTime = 0.1f;

    [Header("Head Tilt Settings")]
    [Tooltip("Maximum tilt angle for the head based on movement.")]
    [SerializeField] private float headTiltAngle = 10f;

    [Header("Upgrade Settings")]
    [Tooltip("Hold duration before opening the upgrade UI.")]
    [SerializeField] private float upgradeHoldDuration = 1.5f;
    #endregion

    #region Private Fields
    private Rigidbody rb;
    private Camera mainCam;
    private PlayerInputActions inputActions;

    private Vector2 moveInput;
    private Vector3 currentMoveDir;
    private Vector3 moveDirVelocity;

    private bool isHoldingUpgrade = false;
    private float upgradeHoldTimer = 0f;

    private Quaternion headInitialLocalRotation;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        mainCam = Camera.main;
        headInitialLocalRotation = headPivot != null ? headPivot.localRotation : Quaternion.identity;

        inputActions = new PlayerInputActions();
        inputActions.Enable();

        inputActions.player.Move.performed += OnMove;
        inputActions.player.Move.canceled += OnMove;
        inputActions.player.Upgrade.performed += OnUpgradePerformed;
        inputActions.player.Upgrade.canceled += OnUpgradeCanceled;
    }

    private void OnDestroy()
    {
        inputActions.player.Move.performed -= OnMove;
        inputActions.player.Move.canceled -= OnMove;
        inputActions.player.Upgrade.performed -= OnUpgradePerformed;
        inputActions.player.Upgrade.canceled -= OnUpgradeCanceled;
        inputActions.Disable();
    }

    private void FixedUpdate()
    {
        MoveAndRollBody();
        TiltHeadBasedOnMovement();
        RotateHeadToMouse();
    }
    #endregion

    #region Input Handlers
    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnUpgradePerformed(InputAction.CallbackContext _)
    {
        if (!ProgressManager.Instance.IsUpgradeAvailable) return;
        isHoldingUpgrade = true;
        upgradeHoldTimer = 0f;
    }

    private void OnUpgradeCanceled(InputAction.CallbackContext _)
    {
        isHoldingUpgrade = false;
        upgradeHoldTimer = 0f;
        UIManager.Instance.SetUpgradeHoldProgress(0f);
    }
    #endregion

    #region Movement & Rolling
    private void MoveAndRollBody()
    {
        // compute desired camera-relative move direction on XZ
        Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 desiredDir = Vector3.zero;
        if (input.sqrMagnitude >= 0.01f)
        {
            Vector3 camF = mainCam.transform.forward; camF.y = 0; camF.Normalize();
            Vector3 camR = mainCam.transform.right; camR.y = 0; camR.Normalize();
            desiredDir = (camF * input.z + camR * input.x).normalized;
        }

        // smooth the direction
        currentMoveDir = Vector3.SmoothDamp(currentMoveDir,
                                           desiredDir,
                                           ref moveDirVelocity,
                                           moveSmoothTime);

        // move root via velocity (preserve gravity)
        Vector3 vel = currentMoveDir * moveSpeed;
        vel.y = rb.velocity.y;
        rb.velocity = vel;

        // roll bodyVisual mesh
        float dist = moveSpeed * Time.fixedDeltaTime;
        if (currentMoveDir.sqrMagnitude > 0.001f)
        {
            float angle = dist / sphereRadius * Mathf.Rad2Deg;
            Vector3 axis = Vector3.Cross(Vector3.up, currentMoveDir).normalized;
            bodyVisual.Rotate(axis, angle, Space.World);
        }
    }
    #endregion

    #region Head Tilt
    private void TiltHeadBasedOnMovement()
    {
        if (headPivot == null || mainCam == null) return;

        // get camera axes
        Vector3 camF = mainCam.transform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = mainCam.transform.right; camR.y = 0; camR.Normalize();

        // extract forward/back and lateral components
        float forward = Vector3.Dot(currentMoveDir, camF);
        float lateral = Vector3.Dot(currentMoveDir, camR);

        // compute pitch and roll
        float pitchAngle = -forward * headTiltAngle; // tilt forward/back
        float rollAngle = -lateral * headTiltAngle;  // tilt left/right

        // apply combined tilt
        headPivot.localRotation = headInitialLocalRotation * Quaternion.Euler(pitchAngle, 0f, rollAngle);
    }
    #endregion

    #region Head Rotation
    private void RotateHeadToMouse()
    {
        if (headVisual == null || mainCam == null) return;

        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        if (ground.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 dir = hitPoint - headVisual.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
            {
                headVisual.rotation = Quaternion.LookRotation(dir, Vector3.up);
            }
        }
    }
    #endregion
}
