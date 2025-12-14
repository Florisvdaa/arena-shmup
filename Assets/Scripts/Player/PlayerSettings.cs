using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections.Generic;
using System.Collections;

public class PlayerSettings : MonoBehaviour
{
    [Header("Default player settings")]
    private float defaultMovementSpeed = 5f;
    private float defaultDashSpeed = 1f;
    private float defaultDashDuation = 1f;
    
    private int defaultHealth = 100;

    // References
    public float DefaultMovementSpeed => defaultMovementSpeed;
    public float DefaultDashSpeed => defaultDashSpeed;
    public float DefaultDashDuration => defaultDashDuation;
    public float DefaultHealth => defaultHealth;
}
