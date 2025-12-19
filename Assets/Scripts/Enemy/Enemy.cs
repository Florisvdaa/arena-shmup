using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{
    // Enemy scriptable object? for the settings?

    [SerializeField] private int health;
    public int Health { get; set; }

    private void Start()
    {
        Health = health;
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
}
