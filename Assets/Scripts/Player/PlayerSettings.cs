using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections.Generic;
using System.Collections;

public class PlayerSettings : MonoBehaviour
{
    [Header("Default player settings")]
    [SerializeField] [Range(1f, 15f)] private float defaultMovementSpeed = 5f;
    [SerializeField] [Range(1f, 100f)] private float defaultAcceleration = 20f;
    [SerializeField] [Range(1f, 100f)] private float defaultDeceleration = 25f;
    [SerializeField] [Range(1f, 100f)] private float defaultDashSpeed = 10f;
    [SerializeField] [Range(0f, 1f)] private float defaultDashDuation = .3f;
    [SerializeField] [Range(0f, 10f)] private float defaultDashCooldown = 1f;

    [SerializeField][Range(5f, 15f)] private float dashMaxDistance = 10f;
    [SerializeField] private float dashSafeOffset = 0.5f;
    [SerializeField] private LayerMask dashObstacles;

    private int defaultHealth = 100;

    // References
    public float DefaultMovementSpeed => defaultMovementSpeed;
    public float DefaultAcceleration => defaultAcceleration;
    public float DefaultDeceleration => defaultDeceleration;
    public float DefaultDashSpeed => defaultDashSpeed;
    public float DefaultDashDuration => defaultDashDuation;
    public float DefaultDashCooldown => defaultDashCooldown;
    public int DefaultHealth => defaultHealth;
    public float DashMaxDistance => dashMaxDistance;
    public float DashSafeOffset => dashSafeOffset;
    public LayerMask DashObstacles => dashObstacles;
}
