using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles random spawning of pickups based on a chance percentage.
/// </summary>
public class PickUpSpawner : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Global access point for the PickUpSpawner instance.
    /// </summary>
    public static PickUpSpawner Instance { get; private set; }
    #endregion

    #region Inspector Fields
    [Header("Pickup Spawn Settings")]
    [Tooltip("List of possible pickup prefabs to spawn.")]
    [SerializeField] private List<GameObject> pickupPrefabs;

    [Tooltip("Chance (0-100%) to spawn a pickup when attempted.")]
    [SerializeField, Range(0f, 100f)] private float spawnChance = 30f;
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize singleton instance or destroy duplicate.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Attempts to spawn a random pickup at the specified position,
    /// based on the configured spawnChance and available prefabs.
    /// </summary>
    /// <param name="spawnPosition">World position where the pickup should appear.</param>
    public void TrySpawnPickup(Vector3 spawnPosition)
    {
        // Roll a random value to determine if a pickup should spawn
        float roll = Random.Range(0f, 100f);
        if (roll <= spawnChance && pickupPrefabs.Count > 0)
        {
            // Choose a random prefab from the list
            GameObject randomPickup = pickupPrefabs[Random.Range(0, pickupPrefabs.Count)];
            // Instantiate the pickup at the given position
            Instantiate(randomPickup, spawnPosition, Quaternion.identity);
        }
    }
    #endregion
}
