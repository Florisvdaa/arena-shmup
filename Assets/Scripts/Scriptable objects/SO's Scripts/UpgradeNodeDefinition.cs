using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/*
 * ScriptableObject to define a node in a permanent skill tree.
 */
public enum UpgradeTree { Speed, Health, Attack }

[CreateAssetMenu(menuName = "Game/Upgrades/UpgradeNodeDefinition")]
public class UpgradeNodeDefinition : ScriptableObject
{
    [Header("Basic Info")]
    public string nodeName;
    public UpgradeTree tree;
    public int tier;
    [Range(1,3)]public int cost;
    [TextArea] public string description;

    [Header("UI Icon")]
    public Sprite icon;

    [Header("Upgrade Effects")]
    [Tooltip("Fired when this upgrade finishes filling")]
    public UpgradeEffect[] effects;

    [Header("Dependencies")]
    [Tooltip("Only unlock this once these nodes are purchased")]
    public UpgradeNodeDefinition[] prerequisites;
}
