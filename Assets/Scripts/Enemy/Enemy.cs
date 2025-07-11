using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [Tooltip("Transform used for enemy visuals (e.g., model or sprite).")]
    [SerializeField] private Transform enemyVisual;
    [Tooltip("Damage dealt to the player on contact.")]
    [SerializeField] private int damage = 1;
    [Tooltip("Starting health of the enemy.")]
    [SerializeField] private int health = 1;
    [Tooltip("Movement speed for NavMeshAgent.")]
    [SerializeField] private float movementSpeed = 3.5f;
    [Tooltip("Score awarded to the player when this enemy dies.")]
    [SerializeField] private int score = 100;
    [Tooltip("Feedback played when the enemy dies.")]
    [SerializeField] private MMF_Player deathParticle;
    [SerializeField] private MMF_Player hitParticle;

    private NavMeshAgent agent;
    private Transform playerTargetTransform;

    /// <summary>
    /// Exposes the serialized "damage" field so other scripts can read/write it.
    /// </summary>
    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    /// <summary>
    /// Invoked when the enemy dies.
    /// </summary>
    public event Action OnDeath;

    /// <summary>
    /// Cache components and initialize movement settings.
    /// </summary>
    private void Awake()
    {
        // Configure NavMeshAgent speed
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = movementSpeed;
        }

        // Get player transform for targeting
        playerTargetTransform = GameManager.Instance.GetPlayerTransform();
    }

    /// <summary>
    /// Update is called once per frame to move toward the player.
    /// </summary>
    private void Update()
    {
        // Move toward player if both agent and target exist
        if (agent != null && playerTargetTransform != null)
        {
            agent.SetDestination(playerTargetTransform.position);
        }
    }

    public void Initialize(float health, float damage)
    {
        this.health += Mathf.RoundToInt(health);
        this.damage += Mathf.RoundToInt(damage);

        if (agent != null)
        {
            agent.speed = movementSpeed;
        }
    }


    /// <summary>
    /// Triggered on collider enter; applies damage to player and handles self-death.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Deal damage to player
            var playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth?.TakeDamage(damage);

            // Die without awarding score
            Die(addScore: false);
        }
    }

    /// <summary>
    /// Applies damage to the enemy and checks for death.
    /// </summary>
    /// <param name="amount">Amount of damage to apply.</param>
    public void TakeDamage(int amount)
    {
        hitParticle.PlayFeedbacks();

        health -= amount;
        if (health <= 0)
        {
            // Die and award score
            Die(addScore: true);
        }
    }

    /// <summary>
    /// Handles death feedback, score, pickups, and death timing.
    /// </summary>
    /// <param name="addScore">Whether to grant score and rewards.</param>
    private void Die(bool addScore)
    {
        // Play death VFX
        deathParticle?.PlayFeedbacks();

        // Stop movement
        if (agent != null)
        {
            agent.isStopped = true;
        }

        if (addScore)
        {
            ScoreManager.Instance.AddScore(score);

            // Track kill and spawn pickup
            KillChainManager.Instance.RegisterKill();
            //PickUpSpawner.Instance.TrySpawnPickup(transform.position);
        }
        else
        {
            // Cancel any ongoing kill chain
            KillChainManager.Instance.CancelKillChain();
        }

        // Delay before final destruction
        Invoke(nameof(DieInvoke), 0.5f);
    }

    /// <summary>
    /// Calculates experience awarded on this enemy's death.
    /// </summary>
    private float CalculateEXP()
    {
        float baseExp = 10f;
        float playerMultiplier = PlayerSettings.Instance.CurrentExpMultiplier;
        float chainMultiplier = KillChainManager.Instance.GetKillChainMultiplier();
        return baseExp * playerMultiplier * chainMultiplier;
    }

    /// <summary>
    /// Final cleanup: invokes death event and destroys this game object.
    /// </summary>
    private void DieInvoke()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
