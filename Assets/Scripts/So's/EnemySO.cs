using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "SO's/EnemySO")]
public class EnemySO : ScriptableObject
{
    public EnemyType enemyType;
    public GameObject enemyPrefab;
    public string enemyName;
    public int enemyHealth;
    public int enemySpeed;
    public int enemyDamage;

    public float detectionRadius;
}

public enum EnemyType
{
    GlitchOrb,              // Default, slow - swarming
    CorruptedShards,        // Fast, kamikaze attackers
    FirewallCrafter,        // Slow, heavy weight
    Spreader,               // Normal but spreads on death
    SpreaderPiece,          // Normal but spreads on death
}
