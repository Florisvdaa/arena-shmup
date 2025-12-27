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
    [SerializeField] protected Transform enemyVisualTransform;
    [SerializeField] protected LayerMask detectionLayer;
    [SerializeField] private GameObject explodeParticle;
    //[SerializeField] protected GameObject hitParticleSystem;
    protected Transform target;
    protected Rigidbody rb;

    protected int health;
    protected int speed;
    protected int damage;
    protected float detectionRadius;
    protected GameObject enemyVisual;

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

            enemyVisual = Instantiate(enemySO.enemyPrefab, enemyVisualTransform.position, Quaternion.identity, enemyVisualTransform);

            SetupParticle();
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
        Instantiate(explodeParticle, transform.position, Quaternion.identity);

        Destroy(this.gameObject);
    }

    private void SetupParticle()
    {
        ParticleSystemRenderer psr = explodeParticle.GetComponent<ParticleSystemRenderer>();

        MeshRenderer enemyRenderer = enemyVisual.GetComponentInChildren<MeshRenderer>();
        Material enemyVisualMat = enemyRenderer.sharedMaterial;

        psr.material = enemyVisualMat;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
