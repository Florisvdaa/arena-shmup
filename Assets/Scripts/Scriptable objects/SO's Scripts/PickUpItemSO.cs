using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PickupType
{
    SpeedBoost,
    HealthBoost,
    FireRateBoost,
    MaxHealthBoost
}
[CreateAssetMenu(fileName = "New Pickup Item", menuName = "Pickup Item", order = 1)]
public class PickUpItemSO : ScriptableObject
{
    public string itemName;
    public PickupType pickupType;
    public float effectAmount;   // How much effect (e.g., speed boost amount)
    public float duration;       // How long the effect lasts
    public Sprite itemIcon;      // Visual representation (like an icon)
    public GameObject itemVisual;


    // You can add more properties if needed (e.g., sound effect, animations)
}
