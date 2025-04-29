using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerSettings playerSettings;

    private void Start()
    {
        playerSettings = GetComponent<PlayerSettings>();

        if (playerSettings == null)
        {
            Debug.LogError("PlayerSettings component not found on Player!");
            return;
        }

        // Make sure player starts at full health
        playerSettings.CurrentHealth = playerSettings.CurrentMaxHealth;

        Debug.Log($"Player starting health: {playerSettings.CurrentHealth}");
    }

    public void TakeDamage(int amount)
    {
        if (playerSettings == null) return;

        playerSettings.CurrentHealth -= amount;
        Debug.Log($"Player took {amount} damage. Current Health: {playerSettings.CurrentHealth}");

        if (playerSettings.CurrentHealth <= 0)
        {
            Debug.Log("Player is dead!");
            // TODO: Add death handling here (e.g., respawn, game over screen)
        }
    }

    public void Heal(int amount)
    {
        if (playerSettings == null) return;

        playerSettings.CurrentHealth += amount;
        Debug.Log($"Player healed {amount}. Current Health: {playerSettings.CurrentHealth}");
    }
}
