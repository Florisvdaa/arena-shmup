using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

/// <summary>
/// Manages the pixel charge effect and spawns the real enemy once charging completes.
/// </summary>
public class PixelEffectController : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("Feedbacks played during the charge effect.")]
    [SerializeField] private MMF_Player chargeFeedback;

    [Tooltip("Enemy prefab instantiated after the charge completes.")]
    [SerializeField] private GameObject enemyPrefab;
    #endregion

    #region Private Fields
    private SpawnManager spawnManager;
    private PixelChargeInEffect charger;
    #endregion

    #region Public Methods
    /// <summary>
    /// Initializes the controller with a SpawnManager reference and starts the charge effect.
    /// </summary>
    /// <param name="manager">SpawnManager instance to register the spawned enemy.</param>
    public void Init(SpawnManager manager)
    {
        spawnManager = manager;
        charger = GetComponentInChildren<PixelChargeInEffect>();
        PlayChargeEffect();
    }

    /// <summary>
    /// Plays the MMF charge feedback and begins cube charging.
    /// </summary>
    public void PlayChargeEffect()
    {
        chargeFeedback?.PlayFeedbacks();
        charger?.StartCharging(this);
    }

    /// <summary>
    /// Instantiates the actual enemy, registers its death callback, and destroys the effect object.
    /// </summary>
    public void TriggerEnemySpawn()
    {
        var go = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        var enemy = go.GetComponent<Enemy>();
        spawnManager.RegisterInstance(enemy);
        Destroy(gameObject);
    }
    #endregion
}
