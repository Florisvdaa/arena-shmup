using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * ScriptableObject to define a node in a permanent skill tree.
 */
public enum UpgradeTree { Speed, Health, Attack }

[CreateAssetMenu(menuName = "Game/Upgrades/UpgradeNodeDefinition")]
public class UpgradeNodeDefinition : ScriptableObject
{
    public string nodeName;
    public UpgradeTree tree;
    public int tier;
    public int cost;
    [TextArea] public string description;
    // You can hook up effects via UnityEvents or custom logic in UpgradeManager
}
