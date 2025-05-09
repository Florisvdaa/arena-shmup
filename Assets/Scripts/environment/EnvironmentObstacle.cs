using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Environtal obstacle that can be placed in the map
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class EnvironmentObstacle : MonoBehaviour
{
    #region Inspector fields
    [Header("Electricity Damage")]
    [Tooltip("Damage per second")]
    [SerializeField] private int damagePerSec = 1;
    [Tooltip("Range of the electricity")]
    [SerializeField] private float damageRange = 3f;
    #endregion

    #region Private fields
    private bool canDamage = true;
    private SphereCollider sphereTrigger;

    // Track which enemies were buffed so we can restore them
    private readonly Dictionary<Enemy, int> buffedEnemies = new Dictionary<Enemy, int>();
    #endregion

    private void Awake()
    {
        // Configure the trigger collider
        sphereTrigger = GetComponent<SphereCollider>();
        sphereTrigger.isTrigger = true;
        sphereTrigger.radius = damageRange;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canDamage) return;

        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // smooth float damage over time
                playerHealth.TakeDamage(damagePerSec * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;

        // Double enemy damage on entry
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null && !buffedEnemies.ContainsKey(enemy))
        {
            buffedEnemies[enemy] = enemy.Damage;
            enemy.Damage = enemy.Damage * 2;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!canDamage) return;

        // Restore enemy damage on exit
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null && buffedEnemies.ContainsKey(enemy))
        {
            enemy.Damage = buffedEnemies[enemy];
            buffedEnemies.Remove(enemy);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }

    /// <summary>
    /// Enable or disable both the damaging and buffing behavior.
    /// </summary>
    public void EnableDamage(bool enable)
    {
        canDamage = enable;
        if (!enable)
        {
            // immediately restore any buffed enemies
            foreach (var kv in buffedEnemies)
                kv.Key.Damage = kv.Value;
            buffedEnemies.Clear();
        }
    }
}
