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
    public static ProgressManager Instance { get; private set; }
    #endregion

    #region Events
    public event Action<int> OnMilestoneReached; // e.g. 1, 2, or 3
    public event Action<int> OnPUPChanged;       // PUP = Power-Up Points
    public event Action OnRoundEnded;
    #endregion

    #region Inspector Fields
    [Tooltip("Total EXP needed to reach all 3 milestones in one round.")]
    [SerializeField] private float roundTotalExp = 100f;

    [Tooltip("XP Bar UI Reference")]
    [SerializeField] private MMProgressBar xpProgressBar;
    #endregion

    #region Private Fields
    private float currentExp = 0f;
    private int milestonesReached = 0;
    private float[] milestoneThresholds;
    private int currentPUP = 0;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        CalculateMilestones();
        ResetRoundProgress();
    }
    #endregion

    #region Public Methods

    public void GainExp(float amount)
    {
        if (milestonesReached >= 3) return;

        currentExp += amount;
        UIManager.Instance.ShowFloatingText(amount.ToString());

        // Handle milestones
        while (milestonesReached < 3 && currentExp >= milestoneThresholds[milestonesReached])
        {
            currentExp -= milestoneThresholds[milestonesReached];
            milestonesReached++;
            currentPUP++;

            OnMilestoneReached?.Invoke(milestonesReached);
            OnPUPChanged?.Invoke(currentPUP);

            // If it's the final milestone, cap bar and exit
            if (milestonesReached >= 3)
            {
                xpProgressBar?.UpdateBar(milestoneThresholds[2], 0f, milestoneThresholds[2]);
                currentExp = 0f; // optionally freeze XP
                return;
            }

            // Reset bar for next milestone
            xpProgressBar?.UpdateBar(0f, 0f, milestoneThresholds[milestonesReached]);
        }

        // Update XP bar toward current milestone
        if (milestonesReached < 3)
        {
            xpProgressBar?.UpdateBar(currentExp, 0f, milestoneThresholds[milestonesReached]);
        }
    }

    /// <summary>
    /// Call this when the round ends.
    /// </summary>
    public void EndRound()
    {
        // Reward 1 permanent skill point
        UpgradeManager.Instance?.AddSkillPoint(1);

        // Pass earned PUPs to PowerUp system
        //PowerUpManager.Instance?.AddPoints(currentPUP);

        // Trigger round feedback/UI
        OnRoundEnded?.Invoke();

        // Reset round-specific values
        ResetRoundProgress();
    }

    public void ResetRoundProgress()
    {
        currentExp = 0f;
        milestonesReached = 0;
        currentPUP = 0;
        CalculateMilestones();

        xpProgressBar?.SetBar(0f, 0f, roundTotalExp);
        OnPUPChanged?.Invoke(currentPUP);
    }

    #endregion

    #region Private Methods
    private void CalculateMilestones()
    {
        milestoneThresholds = new float[3];
        for (int i = 0; i < 3; i++)
        {
            milestoneThresholds[i] = roundTotalExp * ((i + 1) / 3f);
        }
    }
    #endregion

    #region Accessors
    public int GetMilestonesReached() => milestonesReached;
    public float GetCurrentExp() => currentExp;
    public int GetCurrentPUP() => currentPUP;
    public float GetNextMilestoneThreshold() =>
        milestonesReached < 3 ? milestoneThresholds[milestonesReached] : roundTotalExp;
    #endregion
}
