using System.Collections;
using System.Collections.Generic;
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
    public void PlayerDamageFeedback()
    {
        playerDamageFeedback?.PlayFeedbacks();
    }
    #endregion
}
