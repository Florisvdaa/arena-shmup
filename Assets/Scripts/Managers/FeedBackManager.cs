using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using UnityEngine;

/// <summary>
/// Central manager for playing global feedbacks like damage indicators.
/// </summary>
public class FeedBackManager : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Global access point for the FeedBackManager instance.
    /// </summary>
    public static FeedBackManager Instance { get; private set; }
    #endregion

    #region Inspector Fields
    [Header("All Feedbacks")]
    [Tooltip("Feedback to play when the player takes damage.")]
    [SerializeField] private MMF_Player playerDamageFeedback;
    [Tooltip("UI Feedback to play when the player levels up!")]
    [SerializeField] private MMF_Player playerUILevelUpFeedback;
    [Tooltip("Wave indicator")]
    [SerializeField] private MMF_Player waveIndicatorFeedback;
    [Tooltip("Count Down Feedback")]
    [SerializeField] private MMF_Player countDownFeedback;
    [Tooltip("Player teleport Feedback")]
    [SerializeField] private MMF_Player playerTeleportStartPosFeedback;
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize singleton instance or destroy duplicates.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Plays the feedback associated with player damage.
    /// </summary>
    public void PlayerDamageFeedback() => playerDamageFeedback?.PlayFeedbacks();
    public void PlayerUILevelUpFeedback() => playerUILevelUpFeedback?.PlayFeedbacks();
    public void WaveIndicatorFeedback() => waveIndicatorFeedback?.PlayFeedbacks();
    public void PlayCountDownFeedback() => countDownFeedback?.PlayFeedbacks();
    public MMF_Player CountDownPlayer() { return countDownFeedback; }
    public void PlayTeleportToStartPosFeedback() => playerTeleportStartPosFeedback?.PlayFeedbacks();
    #endregion
}
