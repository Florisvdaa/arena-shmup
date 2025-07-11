using System.Collections;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using UnityEngine;

/// <summary>
/// Manages player health, damage feedback, and healing.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    private PlayerSettings playerSettings;

    private void Start()
    {
        playerSettings = GetComponent<PlayerSettings>();
        if (playerSettings == null)
            Debug.LogError("Missing PlayerSettings on Player!");

        //playerSettings.CurrentHealth = playerSettings.CurrentMaxHealth;
    }

    public void TakeDamage(float amount)
    {
        FeedBackManager.Instance.PlayerDamageFeedback();
        playerSettings.CurrentHealth -= amount;
        if (playerSettings.CurrentHealth <= 0)
            Debug.Log("Player is dead!");
    }

    public void Heal(int amount)
    {
        playerSettings.CurrentHealth += amount;
        Debug.Log($"Player healed {amount}. Current Health: {playerSettings.CurrentHealth}");
    }
}
