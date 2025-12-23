using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Base Enemy Settings")]
    [SerializeField] protected EnemySO enemySO;
    [SerializeField] protected LayerMask detectionLayer;
    //[SerializeField] protected GameObject hitParticleSystem;
    protected Transform target;
    protected Rigidbody rb;

    protected int health;
    protected int speed;
    protected int damage;
    protected float detectionRadius;

    public int Health { get; set; }
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (enemySO != null)
        {
            health = enemySO.enemyHealth;
            speed = enemySO.enemySpeed;
            damage = enemySO.enemyDamage;
            detectionRadius = enemySO.detectionRadius;

            Health = health;
        }
    }

    protected virtual void FixedUpdate()
    {
        DetectTarget();
        EnemyBehavior();
    }
    protected abstract void EnemyBehavior();

    private void DetectTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);
        target = hits.Length > 0 ? hits[0].transform : null;
    }

    protected void MoveTowardsTarget(float moveSpeed)
    {
        if (target == null)
            return;

        Vector3 direction = (target.position - transform.position).normalized;
        Vector3 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    public void Damage(int amount)
    {
        //Instantiate(hitParticleSystem,transform.position, Quaternion.identity);

        health -= amount;
        if (health <= 0)
           OnDeath();
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
