using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    [Header("Pick Up SO")]
    [SerializeField] private PickUpItemSO pickupItem;

    [Header("Pick Up Settings")]
    [SerializeField] private Transform pickUpVisualTransform;
    private GameObject pickUpVisual;

    private void Awake()
    {
        pickUpVisual = Instantiate(pickupItem.itemVisual, pickUpVisualTransform);       
    }

    private void OnTriggerEnter(Collider other)
    {
        // is player check via tag? then instance call from player settings.
        PlayerSettings playerSettings = other.GetComponent<PlayerSettings>();
        if (playerSettings != null)
        {
            ApplyPickupEffect(playerSettings);
            // Destroy AFTER setting up, yes, but...
            // make sure the PLAYER handles any coroutines, not this object
            Destroy(gameObject);
        }
    } 

    private void ApplyPickupEffect(PlayerSettings playerSettings)
    {
        switch (pickupItem.pickupType)
        {
            case PickupType.SpeedBoost:
                playerSettings.StartCoroutine(playerSettings.SpeedBoostCoroutine(pickupItem.effectAmount, pickupItem.duration));
                break;
            case PickupType.HealthBoost:
                HealPlayer(playerSettings);
                break;
            case PickupType.FireRateBoost:
                playerSettings.StartCoroutine(playerSettings.FireRateBoostCoroutine(pickupItem.effectAmount, pickupItem.duration));
                break;
            case PickupType.MaxHealthBoost:
                IncreaseMaxHealth(playerSettings);
                break;
        }
    }
    private void HealPlayer(PlayerSettings playerSettings)
    {
        playerSettings.CurrentHealth += (int)pickupItem.effectAmount;
    }

    private void IncreaseMaxHealth(PlayerSettings playerSettings)
    {
        playerSettings.CurrentMaxHealth += (int)pickupItem.effectAmount;
    }
}
