using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player experience, leveling, and upgrade availability.
/// </summary>
public class ProgressManager : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Global access point for the ProgressManager instance.
    /// </summary>
    public static ProgressManager Instance { get; private set; }
    #endregion

    #region Events
    /// <summary>
    /// Invoked when upgrade availability changes. Boolean indicates if upgrades remain.
    /// </summary>
    public event Action<bool> OnUpgradeAvailabilityChanged;
    #endregion

    #region Inspector Fields
    [Tooltip("Base experience required to reach level 2.")]
    [SerializeField] private int baseExpToLevel = 100;

    [Tooltip("Rate at which exp requirement grows each level.")]
    [SerializeField] private float expGrowthRate = 1.2f;
    #endregion

    #region Private Fields
    private int currentLevel = 1;
    private float currentExp = 0f;
    private float expToNextLevel;
    private int availableUpgrades = 0;
    #endregion

    #region Properties
    /// <summary>
    /// Returns true if there are unused upgrades available.
    /// </summary>
    public bool IsUpgradeAvailable => availableUpgrades > 0;
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize singleton and set initial exp required.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        expToNextLevel = baseExpToLevel;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Adds experience and handles level-up logic.
    /// </summary>
    /// <param name="amount">Amount of experience to gain.</param>
    public void GainExp(float amount)
    {
        currentExp += amount;
        UIManager.Instance.ShowFloatingText(amount.ToString());
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    /// <summary>
    /// Consumes one available upgrade.
    /// </summary>
    public void UpgradeConsumed()
    {
        if (availableUpgrades <= 0)
            return;
        availableUpgrades--;
        OnUpgradeAvailabilityChanged?.Invoke(availableUpgrades > 0);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Handles leveling up: increases level, adjusts exp requirement, and flags an upgrade.
    /// </summary>
    private void LevelUp()
    {
        currentLevel++;
        expToNextLevel *= expGrowthRate;
        availableUpgrades++;
        OnUpgradeAvailabilityChanged?.Invoke(true);
    }
    #endregion

    #region References
    /// <summary>
    /// Returns the current player level.
    /// </summary>
    public int GetCurrentLevel() => currentLevel;

    /// <summary>
    /// Returns the current exp progress towards next level.
    /// </summary>
    public float GetCurrentEXP() => currentExp;

    /// <summary>
    /// Returns the exp required to reach the next level.
    /// </summary>
    public float GetEXPTillNextLevel() => expToNextLevel;
    #endregion
}
