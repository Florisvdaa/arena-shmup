using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    // Tracks purchased nodes
    private HashSet<UpgradeNodeDefinition> purchased = new HashSet<UpgradeNodeDefinition>();

    [SerializeField] private int currentSkillPoints = 0;

    /// <summary>
    /// Fires any time a node is newly purchased
    /// </summary>
    public event Action<UpgradeNodeDefinition> OnUpgradePurchased;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // If you want this to persist between scenes:
        // DontDestroyOnLoad(gameObject);
    }

    public void AddSkillPoint(int amount)
    {
        currentSkillPoints += amount;
    }

    /// <summary>
    /// Call when a node finishes filling
    /// </summary>
    public void RegisterPurchase(UpgradeNodeDefinition def)
    {
        if (purchased.Add(def))
            OnUpgradePurchased?.Invoke(def);
    }

    /// <summary>
    /// Has this node been purchased?
    /// </summary>
    public bool IsPurchased(UpgradeNodeDefinition def) => purchased.Contains(def);
}
