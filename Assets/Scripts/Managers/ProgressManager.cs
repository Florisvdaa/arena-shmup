using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
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

    [Header("XP Bar ref")]
    [Tooltip("Drag the MMProgressBar used for XP here")]
    [SerializeField] private MMProgressBar xpProgressBar;
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

        // initialize XP bar (zero fill)
        if (xpProgressBar != null)
        {
            xpProgressBar.SetBar(0f, 0f, expToNextLevel);
        }
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
        // handle one or more level-ups
        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();

            // reset bar to zero on new level
            xpProgressBar?.UpdateBar(0f, 0f, expToNextLevel);
        }

        // finally, update the XP bar to current progress
        xpProgressBar?.UpdateBar(currentExp, 0f, expToNextLevel);
    }

    /// <summary>
    /// Consumes one available upgrade.
    /// </summary>
    public void UpgradeConsumed()
    {
        if (availableUpgrades <= 0) return;
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
        FeedBackManager.Instance.PlayerUILevelUpFeedback();
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
