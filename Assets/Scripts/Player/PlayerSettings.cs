using System.Collections;
using System.Collections.Generic;
//using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

/// <summary>
/// Holds default and current player stats, handles upgrades and boosts.
/// </summary>
public class PlayerSettings : MonoBehaviour
{ 
    #region Singleton
    public static PlayerSettings Instance { get; private set; }
    #endregion

    #region Inspector Fields
    [Header("Default Player Health")]
    [SerializeField] private float defaultMaxHealth = 10f;
    [Header("Default Movement")]
    [SerializeField] private float defaultMovementSpeed = 5f;
    [SerializeField] private Transform playerVisualTransform;
    [Header("Default Weapon")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float defaultFireRate = 0.2f;
    [SerializeField] private float heatPerShot = 10f;
    [SerializeField] private float overheatThreshold = 100f;
    [SerializeField] private float overheatReleaseThreshold = 60f;
    [SerializeField] private float coolRate = 25f;
    [SerializeField] private MMF_Player overheatFeedback;
    [SerializeField] private MMF_Player shootFeedback;
    [Header("Default EXP")]
    [SerializeField] private float defaultExpMultiplier = 1f;
    [Header("Upgrade Feedback")]
    [SerializeField] private MMF_Player upgradeFeedback;
    [Header("HealthBar ref")]
    [SerializeField] private MMProgressBar progressBar;
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCoolDown = 1f;
    #endregion

    #region Runtime Fields & Properties
    private float currentMaxHealth;
    private float currentHealth;
    private float currentMovementSpeed;
    private float currentFireRate;
    private float currentExpMultiplier;

    public float CurrentMaxHealth
    {
        get => currentMaxHealth;
        set
        {
            currentMaxHealth = Mathf.Max(1f, value);
            // Make sure the bar refreshes
            RefreshHealthBar();
        }
    }
    public float CurrentHealth
    {
        get => currentHealth;
        set  /*=> currentHealth = Mathf.Clamp(value, 0f, CurrentMaxHealth);*/
        {
            // clamp
            currentHealth = Mathf.Clamp(value, 0f, CurrentMaxHealth);
            // drive the bar
            if (progressBar != null)
            {
                progressBar.UpdateBar(currentHealth, 0f, CurrentMaxHealth);
                // or, if you prefer to hand it a normalized 0–1 value:
                // float n = currentHealth / CurrentMaxHealth;
                // progressBar.UpdateBar01(n);
            }
        }
    }
    public float CurrentMovementSpeed { get => currentMovementSpeed; set => currentMovementSpeed = Mathf.Max(0f, value); }
    public float CurrentFireRate { get => currentFireRate; set => currentFireRate = Mathf.Max(0.01f, value); }
    public float CurrentExpMultiplier { get => currentExpMultiplier; set => currentExpMultiplier = Mathf.Max(1f, value); }

    public float DashSpeed => dashSpeed;
    public float DashDuration => dashDuration;
    public float DashCooldown => dashCoolDown;
    public float HeatPerShot => heatPerShot;
    public float OverheatThreshold => overheatThreshold;
    public float OverheatReleaseThreshold => overheatReleaseThreshold;
    public float CoolRate => coolRate;
    public Transform PlayerVisualTransform => playerVisualTransform;
    public Transform FirePoint => firePoint;
    public MMF_Player ShootFeedback => shootFeedback;
    public MMF_Player UpgradeFeedback => upgradeFeedback;
    public MMF_Player OverheatFeedback => overheatFeedback;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetToDefault();
        }
        else Destroy(gameObject);
    }
    #endregion

    #region Default Reset
    public void ResetToDefault()
    {
        CurrentMaxHealth = defaultMaxHealth;
        CurrentHealth = defaultMaxHealth;
        CurrentMovementSpeed = defaultMovementSpeed;
        CurrentFireRate = defaultFireRate;
        CurrentExpMultiplier = defaultExpMultiplier;
    }

    public void RefreshHealthBar()
    {
        if (progressBar != null)
        {
            progressBar.UpdateBar(currentHealth, 0f, currentMaxHealth);
        }
    }
    #endregion

    #region Stat Upgrades
    public void IncreaseSpeed(float amount)
    {
        CurrentMovementSpeed += amount;
        upgradeFeedback?.PlayFeedbacks();
    }
    public void IncreaseFireRate(float amount)
    {
        CurrentFireRate = Mathf.Max(0.01f, CurrentFireRate - amount);
        upgradeFeedback?.PlayFeedbacks();
    }
    public void IncreaseHealth(float amount)
    {
        CurrentMaxHealth += amount;
        CurrentHealth += amount; // This will now re-trigger bar update
        RefreshHealthBar(); // Redundant, but ensures bar uses new max
        upgradeFeedback?.PlayFeedbacks();

        Debug.Log($"Current max health: {currentMaxHealth} / Current Health: {currentHealth}");
    }
    public void IncreaseHealthByPercentage(float percentage)
    {
        float amount = currentMaxHealth * percentage;
        IncreaseHealth(amount); // already refreshes
    }
    public void IncreaseExpMultiplier(float amount)
    {
        CurrentExpMultiplier += amount;
        upgradeFeedback?.PlayFeedbacks();
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
