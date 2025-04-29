using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    [Header("Default Player Health Settings")]
    [SerializeField] private int defaultMaxHealth = 3;

    [Header("Default Player Movement Settings")]
    [SerializeField] private float defaultMovementSpeed = 5f;
    [SerializeField] private Transform playerVisualTransform;

    [Header("Default Player Weapon Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float defaultFireRate = 0.2f;
    [SerializeField] private MMF_Player shootFeedback;

    // Current (runtime) values
    private int currentMaxHealth;
    private int currentHealth;
    private float currentMovementSpeed;
    private float currentFireRate;

    #region Properties

    // Expose Default values (readonly)
    public int DefaultMaxHealth => defaultMaxHealth;
    public float DefaultMovementSpeed => defaultMovementSpeed;
    public Transform PlayerVisualTransform => playerVisualTransform;
    public Transform FirePoint => firePoint;
    public float DefaultFireRate => defaultFireRate;
    public MMF_Player ShootFeedback => shootFeedback;

    // Expose Current values (read/write)
    public int CurrentMaxHealth
    {
        get => currentMaxHealth;
        set => currentMaxHealth = Mathf.Max(1, value); // Always at least 1
    }

    public int CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0, currentMaxHealth); // Always between 0 and max
    }

    public float CurrentMovementSpeed
    {
        get => currentMovementSpeed;
        set => currentMovementSpeed = Mathf.Max(0f, value); // No negative speed
    }

    public float CurrentFireRate
    {
        get => currentFireRate;
        set => currentFireRate = Mathf.Max(0.01f, value); // Prevent division by 0 later
    }
    #endregion

    private void Awake()
    {
        // Always start full
        ResetToDefault();
    }

    public void ResetToDefault()
    {
        currentMaxHealth = defaultMaxHealth;
        currentHealth = defaultMaxHealth;
        currentMovementSpeed = defaultMovementSpeed;
        currentFireRate = defaultFireRate;
    }

    #region Modifiers Coroutines
    public IEnumerator SpeedBoostCoroutine(float amount, float duration)
    {
        CurrentMovementSpeed += amount;

        yield return new WaitForSeconds(duration);

        CurrentMovementSpeed -= amount;
    }

    public IEnumerator FireRateBoostCoroutine(float amount, float duration)
    {
        CurrentFireRate -= amount;

        yield return new WaitForSeconds(duration);

        CurrentFireRate += amount;
    }
    #endregion
}
