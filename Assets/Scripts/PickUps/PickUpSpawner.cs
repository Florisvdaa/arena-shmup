using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    public static PickUpSpawner Instance { get; private set; }

    [Header("Pickup Spawn Settings")]
    [SerializeField] private List<GameObject> pickupPrefabs;
    [SerializeField, Range(0, 100)] private float spawnChance = 30f; // 30% default chance

    private void Awake()
    {
        // Singleton pattern (simple)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Attempts to spawn a random pickup at a given position.
    /// </summary>
    public void TrySpawnPickup(Vector3 spawnPosition)
    {
        float roll = Random.Range(0f, 100f);
        if (roll <= spawnChance && pickupPrefabs.Count > 0)
        {
            GameObject randomPickup = pickupPrefabs[Random.Range(0, pickupPrefabs.Count)];
            Instantiate(randomPickup, spawnPosition, Quaternion.identity);
        }
    }
}
