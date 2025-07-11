using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using System.Collections.Generic;
using System.Collections;

public class PlayerSettings : MonoBehaviour
{
    public static PlayerSettings Instance { get; private set; }

    [Header("Default Player Health")]
    [SerializeField] private float defaultMaxHealth = 10f;

    [Header("Default Movement")]
    [SerializeField] private float defaultMovementSpeed = 5f;
    [SerializeField] private Transform playerVisualTransform;

    [Header("Default Weapon")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float defaultFireRate = 0.2f;
    [SerializeField] private float minFireRate = 0.05f;
    [SerializeField] private float defaultFireDamage = 1f;
    [SerializeField] private MMF_Player shootFeedback;

    [Header("Default EXP")]
    [SerializeField] private float defaultExpMultiplier = 1f;

    [Header("Upgrade Feedback")]
    [SerializeField] private MMF_Player upgradeFeedback;

    [Header("HealthBar ref")]
    [SerializeField] private MMProgressBar progressBar;
    [SerializeField] private RectTransform progressBarRectTransform;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCoolDown = 1f;

    [Header("Default Defense")]
    [SerializeField] private float defaultDefenseMultiplier = 0f;

    // Runtime fields
    private float currentMaxHealth;
    private float currentHealth;
    private float currentMovementSpeed;
    private float currentFireRate;
    private float currentFireDamage;
    private float currentExpMultiplier;
    private float currentDefenseMultiplier;

    private Vector3 progressBarRotation;

    public float CurrentMaxHealth
    {
        get => currentMaxHealth;
        set
        {
            currentMaxHealth = Mathf.Max(1f, value);
            RefreshHealthBar();
        }
    }

    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0f, CurrentMaxHealth);
            progressBar?.UpdateBar(currentHealth, 0f, CurrentMaxHealth);
        }
    }

    public float CurrentMovementSpeed { get => currentMovementSpeed; set => currentMovementSpeed = Mathf.Max(0f, value); }
    public float CurrentFireRate { get => currentFireRate; set => currentFireRate = Mathf.Clamp(value, minFireRate, 999f); }
    public float CurrentFireDamage { get => currentFireDamage; set => currentFireDamage = Mathf.Max(0f, value); }
    public float CurrentExpMultiplier { get => currentExpMultiplier; set => currentExpMultiplier = Mathf.Max(1f, value); }
    public float CurrentDefenseMultiplier => currentDefenseMultiplier;

    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCoolDown;
    public float MinFireRate => minFireRate;
    public Transform PlayerVisualTransform => playerVisualTransform;
    public Transform FirePoint => firePoint;
    public MMF_Player ShootFeedback => shootFeedback;
    public MMF_Player UpgradeFeedback => upgradeFeedback;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetToDefault();
        }
        else Destroy(gameObject);
    }

    public void ResetToDefault()
    {
        CurrentMaxHealth = defaultMaxHealth;
        CurrentHealth = defaultMaxHealth;
        CurrentMovementSpeed = defaultMovementSpeed;
        CurrentFireRate = defaultFireRate;
        CurrentFireDamage = defaultFireDamage;
        CurrentExpMultiplier = defaultExpMultiplier;
        currentDefenseMultiplier = defaultDefenseMultiplier;
    }

    public void RefreshHealthBar()
    {
        if (progressBar != null)
        {
            progressBar.UpdateBar(currentHealth, 0f, currentMaxHealth);
            progressBarRectTransform.Rotate(progressBarRotation);
        }
    }

    // Upgrade methods
    public void IncreaseSpeed(float amount)
    {
        CurrentMovementSpeed += amount;
        upgradeFeedback?.PlayFeedbacks();
    }

    public void IncreaseFireRate(float amount)
    {
        if (CurrentFireRate <= minFireRate)
        {
            Debug.Log("Fire rate already maxed out!");
            return;
        }

        CurrentFireRate = Mathf.Max(minFireRate, CurrentFireRate - amount);
        upgradeFeedback?.PlayFeedbacks();
    }

    public void IncreaseHealth(float amount)
    {
        CurrentMaxHealth += amount;
        CurrentHealth += amount;
        RefreshHealthBar();
        upgradeFeedback?.PlayFeedbacks();
    }

    public void IncreaseHealthByPercentage(float percentage)
    {
        float amount = currentMaxHealth * percentage;
        IncreaseHealth(amount);
    }

    public void IncreaseExpMultiplier(float amount)
    {
        CurrentExpMultiplier += amount;
        upgradeFeedback?.PlayFeedbacks();
    }

    public void IncreaseDashLength(float extraDuration)
    {
        dashDuration += extraDuration;
        upgradeFeedback?.PlayFeedbacks();
    }

    public void IncreaseFireDamage(float amount)
    {
        CurrentFireDamage += amount;
        upgradeFeedback?.PlayFeedbacks();
    }

    public void IncreaseDefense(float percentageReduction)
    {
        currentDefenseMultiplier += percentageReduction;
        currentDefenseMultiplier = Mathf.Clamp(currentDefenseMultiplier, 0f, 0.9f);
        upgradeFeedback?.PlayFeedbacks();
    }

    public IEnumerator SpeedBoostCoroutine(float amount, float duration)
    {
        CurrentMovementSpeed += amount;
        yield return new WaitForSeconds(duration);
        CurrentMovementSpeed -= amount;
    }

    public IEnumerator FireRateBoostCoroutine(float amount, float duration)
    {
        CurrentFireRate = Mathf.Max(minFireRate, CurrentFireRate - amount);
        yield return new WaitForSeconds(duration);
        CurrentFireRate = Mathf.Min(1f, CurrentFireRate + amount); // or store original value if needed
    }
}
