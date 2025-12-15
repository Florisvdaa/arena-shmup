using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections.Generic;
using System.Collections;

public class PlayerSettings : MonoBehaviour
{
    [Header("Default player settings")]
    private float defaultMovementSpeed = 5f;
    private float defaultAcceleration = 20f;
    private float defaultDeceleration = 25f;
    private float defaultDashSpeed = 10f;
    private float defaultDashDuation = .3f;
    private float defaultDashCooldown = 1f;
    
    private int defaultHealth = 100;

    // References
    public float DefaultMovementSpeed => defaultMovementSpeed;
    public float DefaultAcceleration => defaultAcceleration;
    public float DefaultDeceleration => defaultDeceleration;
    public float DefaultDashSpeed => defaultDashSpeed;
    public float DefaultDashDuration => defaultDashDuation;
    public float DefaultDashCooldown => defaultDashCooldown;
    public float DefaultHealth => defaultHealth;
}
