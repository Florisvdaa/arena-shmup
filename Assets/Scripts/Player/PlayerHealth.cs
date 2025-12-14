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
    }

    public void Heal(int amount)
    {
    }
}
