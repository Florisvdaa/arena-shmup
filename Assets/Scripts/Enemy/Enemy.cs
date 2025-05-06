using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private Transform enemyVisual;
    [SerializeField] private int damage = 1;
    [SerializeField] private int health = 1;
    [SerializeField] private float movementSpeed = 3.5f;
    [SerializeField] private int score = 100;
    private NavMeshAgent agent;
    private Transform playerTargetTransform;

    [SerializeField] private MMF_Player deathParticle;
    public event Action OnDeath;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = movementSpeed;
        }

        playerTargetTransform = GameManager.Instance.GetPlayerTransform();
    }
    private void Update()
    {
        if (playerTargetTransform != null && agent != null)
        {
            agent.SetDestination(playerTargetTransform.position);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Die(false); // Die WITHOUT giving score
        }
    }
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die(true); // Die WITH giving score
        }
    }
    private void Die(bool addScore)
    {
        deathParticle.PlayFeedbacks();
        movementSpeed = 0;

        if (addScore)
        {
            KillChainManager.Instance.RegisterKill();
            PickUpSpawner.Instance.TrySpawnPickup(transform.position); // First try spawn pick up then die
            ProgressManager.Instance.GainExp(CalculateEXP());
        }
        else
        {
            KillChainManager.Instance.CancelKillChain();
        }

        Invoke(nameof(DieInvoke), 0.5f);
    }

    private float CalculateEXP()
    {
        float baseExp = 10f;
        float playerExpMultiplier = PlayerSettings.Instance.CurrentExpMultiplier;
        float killChainMultiplier = KillChainManager.Instance.GetKillChainMultiplier();

        float totalExp = baseExp * playerExpMultiplier * killChainMultiplier;

        return totalExp;
    }

    private void DieInvoke()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
