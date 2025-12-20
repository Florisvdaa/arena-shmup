using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Settings")]
    [SerializeField] private EnemySO enemySO;
    [SerializeField] private LayerMask detectionLayer;

    private EnemyType enemyType;
    private GameObject enemyPrefab;
    private string enemyName;
    private int enemyHealth;
    private int enemySpeed;
    private int enemyDamage;
    private float detectionRadius;

    private float stopDistance = 1.2f;

    private Transform target;
    private Rigidbody rb;
    
    public int Health { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (enemySO != null)
        {
            enemyType = enemySO.enemyType;
            //enemyPrefab = enemySO.enemyPrefab;
            enemyName = enemySO.enemyName;
            enemyHealth = enemySO.enemyHealth;
            enemySpeed = enemySO.enemySpeed;
            enemyDamage = enemySO.enemyDamage;
            detectionRadius = enemySO.detectionRadius;

            Health = enemyHealth;
        }
    }

    private void FixedUpdate()
    {
        EnemyBehavior();
    }

    private void DetectTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        target = hits.Length > 0 ? hits[0].transform : null;
    }

    private void MoveTowardsTarget()
    {
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= stopDistance)
            return;

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 newPosition = rb.position + direction * enemySpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    private void EnemyBehavior()
    {
        switch(enemyType)
        {
            case EnemyType.GlitchOrb:
                DetectTarget();
                if (target != null)
                    MoveTowardsTarget();
                break;
            case EnemyType.CorruptedShards:
                break;
            case EnemyType.FirewallCrafter:
                break;
            case EnemyType.Spreader:
                break;
            case EnemyType.SpreaderPiece:
                break;
        }
    }
    public void Damage(int amount)
    {
        Health -= amount;

        if(Health <= 0)
            OnDeath();
        else
            // take damage feedback
            Debug.Log($"took {amount} damage, new health {Health}");
    }

    public void OnDeath()
    {
        Destroy(this.gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
