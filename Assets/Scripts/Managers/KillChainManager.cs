using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks kill chains to apply dynamic score multipliers based on rapid kills.
/// </summary>
public class KillChainManager : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Global access point for the KillChainManager instance.
    /// </summary>
    public static KillChainManager Instance { get; private set; }
    #endregion

    #region Inspector Fields
    [Header("Kill Chain Settings")]
    [Tooltip("Time (seconds) after which the kill chain resets.")]
    [SerializeField] private float chainResetTime = 2.5f;
    [Tooltip("Multiplier increase per kill in the chain.")]
    [SerializeField] private float multiplierIncrease = 0.25f;
    [Tooltip("Maximum kill chain multiplier allowed.")]
    [SerializeField] private float maxMultiplier = 3f;
    #endregion

    #region Private Fields
    private float currentMultiplier = 1f;
    private float lastKillTime = 0f;
    private int killCount = 0;
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize singleton.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Resets chain if time since last kill exceeds reset threshold.
    /// </summary>
    private void Update()
    {
        if (Time.time - lastKillTime > chainResetTime)
        {
            currentMultiplier = 1f;
            killCount = 0;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Registers a kill, updating the multiplier based on chain.
    /// </summary>
    public void RegisterKill()
    {
        killCount++;
        lastKillTime = Time.time;
        currentMultiplier = Mathf.Min(1f + killCount * multiplierIncrease, maxMultiplier);
    }

    /// <summary>
    /// Cancels the current kill chain and resets multiplier.
    /// </summary>
    public void CancelKillChain()
    {
        currentMultiplier = 1f;
        killCount = 0;
    }
    #endregion

    #region References
    /// <summary>
    /// Returns the current kill chain multiplier.
    /// </summary>
    public float GetKillChainMultiplier() => currentMultiplier;
    #endregion
}
