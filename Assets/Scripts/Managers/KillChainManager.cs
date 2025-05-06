using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillChainManager : MonoBehaviour
{
    public static KillChainManager Instance {  get; private set; }

    [Header("KillChain Settings")]
    [SerializeField] private float chainResetTime = 2.5f;
    [SerializeField] private float multiplierIncrease = 0.25f;
    [SerializeField] private float maxMultiplier = 3f;

    private float currentMultiplier = 1f;
    private float lastKillTime;
    private int killCount;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Update()
    {
        if (Time.time - lastKillTime > chainResetTime)
        {
            currentMultiplier = 1f;
            killCount = 0;
        }
    }

    public void RegisterKill()
    {
        killCount++;
        lastKillTime = Time.time;

        currentMultiplier = Mathf.Min(1f + killCount * multiplierIncrease, maxMultiplier);
    }
    public void CancelKillChain()
    {
        currentMultiplier = 1f;
        killCount = 0;
    }

    #region References
    public float GetKillChainMultiplier() => currentMultiplier;
    #endregion
}
