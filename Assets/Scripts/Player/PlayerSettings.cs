using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public static PlayerSettings Instance { get; private set; }

    [Header("Default Player Health Settings")]
    [SerializeField] private float defaultMaxHealth = 100f;

    [Header("Default Player Movement Settings")]
    [SerializeField] private float defaultMovementSpeed = 5f;
    [SerializeField] private Transform playerVisualTransform;

    [Header("Default Player Weapon Settings")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float defaultFireRate = 0.2f;
    [SerializeField] private MMF_Player shootFeedback;

    [Header("Default EXP Settings")]
    [SerializeField] private float defaultExpMultiplier = 1f;

    [Header("Upgrade Feedback (Optional)")]
    [SerializeField] private MMF_Player upgradeFeedback;

    // Current (runtime) values
    private float currentMaxHealth;
    private float currentHealth;
    private float currentMovementSpeed;
    private float currentFireRate;
    private float currentExpMultiplier;

    #region Properties

    // Default values
    public float DefaultMaxHealth => defaultMaxHealth;
    public float DefaultMovementSpeed => defaultMovementSpeed;
    public float DefaultFireRate => defaultFireRate;
    public float DefaultExpMultiplier => defaultExpMultiplier;
    public Transform PlayerVisualTransform => playerVisualTransform;
    public Transform FirePoint => firePoint;
    public MMF_Player ShootFeedback => shootFeedback;

    // Runtime values (with validation)
    public float CurrentMaxHealth
    {
        get => currentMaxHealth;
        set => currentMaxHealth = Mathf.Max(1f, value); // now float
    }

    public float CurrentHealth
    {
        get => currentHealth;
        set => currentHealth = Mathf.Clamp(value, 0f, CurrentMaxHealth);
    }

    public float CurrentMovementSpeed
    {
        get => currentMovementSpeed;
        set => currentMovementSpeed = Mathf.Max(0f, value);
    }

    public float CurrentFireRate
    {
        get => currentFireRate;
        set => currentFireRate = Mathf.Max(0.01f, value);
    }

    public float CurrentExpMultiplier
    {
        get => currentExpMultiplier;
        set => currentExpMultiplier = Mathf.Max(1f, value);
    }

    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetToDefault();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetToDefault()
    {
        CurrentMaxHealth = defaultMaxHealth;
        CurrentHealth = defaultMaxHealth;
        CurrentMovementSpeed = defaultMovementSpeed;
        CurrentFireRate = defaultFireRate;
        CurrentExpMultiplier = defaultExpMultiplier;
    }

    #region Stat Upgrades

    public void IncreaseSpeed(float amount)
    {
        CurrentMovementSpeed += amount;
        PlayUpgradeFeedback();
    }

    public void IncreaseFireRate(float amount)
    {
        CurrentFireRate -= amount;
        PlayUpgradeFeedback();
    }

    public void IncreaseHealth(float amount)
    {
        CurrentMaxHealth += amount;
        CurrentHealth = CurrentMaxHealth; // heal to full
        PlayUpgradeFeedback();
    }

    public void IncreaseExpMultiplier(float amount)
    {
        CurrentExpMultiplier += amount;
        PlayUpgradeFeedback();
    }

    private void PlayUpgradeFeedback()
    {
        if (upgradeFeedback != null)
        {
            upgradeFeedback.PlayFeedbacks();
        }
    }

    #endregion

    #region Timed Boosts

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
