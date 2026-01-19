using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    [Header("Player Movement settings")]
    [SerializeField] private GameObject playerVisual;
    private Rigidbody rb;

    private PlayerInputActions inputActions;

    public int Health { get; set; }
    public int MaxHealth { get; set; }

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
    private float dashMaxDistance;
    private float dashSafeOffset;
    private LayerMask dashObstacles;


    private PlayerSettings playerSettings;
    private MeshTrail playerMeshTrail;
    private PlayerWeapon playerWeapon;

    private Animator playerAnimator;
    private int velocityHash;
    private int forwardHash;
    private int strafeHash;


    private bool isFireHolding;

    private bool playerCanMove = true;

    // UI References
    public bool CanDash => canDash;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        playerSettings = GetComponent<PlayerSettings>();
        playerWeapon = GetComponent<PlayerWeapon>();
        playerMeshTrail = GetComponent<MeshTrail>();
        rb = GetComponent<Rigidbody>();

        playerAnimator = GetComponentInChildren<Animator>();
        velocityHash = Animator.StringToHash("Velocity");
        forwardHash = Animator.StringToHash("MoveZ");
        strafeHash = Animator.StringToHash("MoveX");
        ChangeStats();
    }

    private void Start()
    {
        inputActions.player.Dash.performed += ctx => HandleDash();
    }

    private void FixedUpdate()
    {
        if (!playerCanMove)
            return;

        if (!isDashing)
            TryMovement();


        UpdateAnimator();
        //playerAnimator.SetBool("IsShooting", isFireHolding);
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
            ref currentVelocity, // reuse as velocity ref is fine or use a separate ref var
            (moveDir.sqrMagnitude > 0.001f) ? (1f / acceleration) : (1f / deceleration),
            Mathf.Infinity,
            Time.fixedDeltaTime
        );

        if (moveDir.sqrMagnitude < 0.001f)
        {
            currentVelocity = Vector3.zero;
        }


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
        if (!playerCanMove || !canDash || isDashing)
            return;

        // Determine dash direction
        Vector2 moveInput = inputActions.player.Move.ReadValue<Vector2>();
        Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y);

        if (dir.sqrMagnitude < 0.01f)
            dir = playerVisual.transform.forward;

        dashDirection = dir.normalized;

        StartCoroutine(TeleportDash());

        //if (!playerCanMove) return;

        ////Debug.Log("Dash");
        //if (!canDash || isDashing) return;

        //// Decide the direction to dash in.
        //Vector2 moveInput = inputActions.player.Move.ReadValue<Vector2>();
        //Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y);

        //if (dir.sqrMagnitude < 0.01f)
        //{
        //    // fallback -> dash in facing direciotn
        //    dir = playerVisual.transform.forward;
        //}
        //dashDirection = dir.normalized;

        //StartCoroutine(DashCoroutine());
    }
    private IEnumerator TeleportDash()
    {
        isDashing = true;
        canDash = false;
        DashCooldownProgress = 1f;

        rb.velocity = Vector3.zero;

        //playerMeshTrail.StartTrailCoroutine(dashDuation);

        Vector3 startPos = transform.position;
        Vector3 forward = playerVisual.transform.forward;

        // 1. Cast ray forward
        if (Physics.Raycast(startPos, forward, out RaycastHit hit, dashMaxDistance, dashObstacles))
        {
            float hitDistance = hit.distance;

            // 2. Check if we can teleport THROUGH the wall
            Vector3 behindWallPos = startPos + forward * (hitDistance + dashSafeOffset);

            // Cast again from behind the wall to see if it's free
            if (!Physics.CheckSphere(behindWallPos, 0.4f, dashObstacles))
            {
                // Teleport THROUGH the wall
                transform.position = behindWallPos;
            }
            else
            {
                // Teleport TO the wall hit point (minus offset)
                Vector3 safePos = hit.point - forward * dashSafeOffset;
                transform.position = safePos;
            }
        }
        else
        {
            // 3. No wall -> teleport full distance
            transform.position = startPos + forward * dashMaxDistance;
        }

        // Dash duration is only for visuals
        yield return new WaitForSeconds(dashDuation);

        isDashing = false;

        // Cooldown timer
        float timer = 0f;
        while (timer < dashCooldown)
        {
            timer += Time.deltaTime;
            DashCooldownProgress = 1f - (timer / dashCooldown);
            yield return null;
        }

        DashCooldownProgress = 0f;
        canDash = true;

    }
    private IEnumerator DashCoroutine()
    {
        isDashing = true;
        canDash = false;

        rb.velocity = Vector3.zero;

        playerMeshTrail.StartTrailCoroutine(dashDuation);

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

    private void UpdateAnimator()
    {
        Vector3 flatVel = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        float speed = flatVel.magnitude;

        Vector3 dir = flatVel.normalized;

        float forward = Vector3.Dot(dir, playerVisual.transform.forward);
        float strafe = Vector3.Dot(dir, playerVisual.transform.right);

        // Deadzone
        if (speed < 0.05f)
        {
            forward = 0f;
            strafe = 0f;
            speed = 0f;
        }

        // Apply directional deadzones
        //if (Mathf.Abs(forward) < 0.05f) forward = 0f;
        //if (Mathf.Abs(strafe) < 0.05f) strafe = 0f;

        playerAnimator.SetFloat(forwardHash, forward, 0.1f, Time.deltaTime);
        playerAnimator.SetFloat(strafeHash, strafe, 0.1f, Time.deltaTime);
        playerAnimator.SetFloat(velocityHash, speed, 0.1f, Time.deltaTime);
    }

    public void Damage(int amount) 
    {
        Health -= amount;

        Debug.Log($"Damage: {amount} new health: {Health}");
    }
    public void OnDeath()
    {
        // Player is dead.
    }

    public void ChangeStats()
    {
        if (playerSettings != null)
        {
            movementSpeed = playerSettings.DefaultMovementSpeed;
            acceleration = playerSettings.DefaultAcceleration;
            deceleration = playerSettings.DefaultDeceleration;
            dashSpeed = playerSettings.DefaultDashSpeed;
            dashDuation = playerSettings.DefaultDashDuration;
            dashCooldown = playerSettings.DefaultDashCooldown;

            dashMaxDistance = playerSettings.DashMaxDistance;
            dashObstacles = playerSettings.DashObstacles;
            dashSafeOffset = playerSettings.DashSafeOffset;

            MaxHealth = playerSettings.DefaultHealth;
            Health = playerSettings.DefaultHealth;
        }
    }

    private void OnEnable()
    {
        inputActions.player.Shoot.performed += ctx => isFireHolding = true;
        inputActions.player.Shoot.canceled += ctx => isFireHolding = false;
        //inputActions.player.Reload.performed += ctx => playerWeapon.StartCooldown();

        inputActions.Enable();   
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void ToggleMovement() => playerCanMove = !playerCanMove;


    public bool PlayerCanMove => playerCanMove;
    public bool IsFireHolding => isFireHolding;
    public float DashCooldownProgress { get; private set; } // 0 = ready, 1 = full cooldown
    public void IncreaseMovementSpeed(float amount)
    {
        movementSpeed += amount;
    }
}

