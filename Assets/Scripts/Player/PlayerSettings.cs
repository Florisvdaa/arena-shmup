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
    [Header("Visual Transforms")]
    [Tooltip("Sphere mesh that rolls around.")]
    [SerializeField] private Transform bodyVisualTransform;
    [Tooltip("Pivot used for head tilt.")]
    [SerializeField] private Transform headPivotTransform;
    [Tooltip("Actual head mesh that looks at the cursor.")]
    [SerializeField] private Transform headVisualTransform;

    [Header("Default Player Health")]
    [SerializeField] private float defaultMaxHealth = 10f;

    [Header("Default Movement")]
    [SerializeField] private float defaultMovementSpeed = 5f;
    [SerializeField] private Transform playerVisualTransform;

    [Header("Default Weapon")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float defaultFireRate = 0.2f;
    [SerializeField] private MMF_Player shootFeedback;

    [Header("Default EXP")]
    [SerializeField] private float defaultExpMultiplier = 1f;

    [Header("Upgrade Feedback")]
    [SerializeField] private MMF_Player upgradeFeedback;

    [Header("HealthBar ref")]
    [SerializeField] private MMProgressBar progressBar;
    #endregion

    #region Runtime Fields & Properties
    private float currentMaxHealth;
    private float currentHealth;
    private float currentMovementSpeed;
    private float currentFireRate;
    private float currentExpMultiplier;

    public float CurrentMaxHealth { get => currentMaxHealth; set => currentMaxHealth = Mathf.Max(1f, value); }
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

    public Transform BodyVisualTransform => bodyVisualTransform;
    public Transform HeadPivotTransform => headPivotTransform;
    public Transform HeadVisualTransform => headVisualTransform;
    public Transform FirePoint => firePoint;
    public MMF_Player ShootFeedback => shootFeedback;
    public MMF_Player UpgradeFeedback => upgradeFeedback;
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
        CurrentHealth = CurrentMaxHealth;
        upgradeFeedback?.PlayFeedbacks();
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
