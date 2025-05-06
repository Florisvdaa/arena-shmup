using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    [SerializeField] private int baseExpToLevel = 100;
    [SerializeField] private float expGrowthRate = 1.2f;

    private int currentLevel = 1;
    private float currentExp = 0;
    private float expToNextLevel;
    private bool upgradeAvailable = false;
    private int availableUpgrades = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        expToNextLevel = baseExpToLevel;
    }

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

    private void LevelUp()
    {
        currentLevel++;
        expToNextLevel *= expGrowthRate;
        upgradeAvailable = true;
        availableUpgrades++;
        // Trigger upgrade UI or reward player
    }
    public void UpgradeConsumed()
    {
        availableUpgrades--;
        if (availableUpgrades == 0)
        {
            upgradeAvailable = false;
        }
        else
        {
            upgradeAvailable = true;
        }
    }
    #region References
    public int GetCurrentLevel() => currentLevel;
    public float GetCurrentEXP() => currentExp;
    public float GetEXPTillNextLevel() => expToNextLevel; 
    public bool IsUpgradeAvailable() => upgradeAvailable;
    #endregion
}
