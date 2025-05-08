using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Singleton
    public static SpawnManager Instance { get; private set; }
    #endregion

    #region Fields
    [Header("Spawn Settings")]
    [Tooltip("List of spawn points for enemies.")]
    [SerializeField] private List<Transform> spawnTransformList = new List<Transform>();
    [Tooltip("Prefab to use for the final enemy object.")]
    [SerializeField] private GameObject enemyPrefab;
    [Tooltip("Prefab used for pixel charge effect.")]
    [SerializeField] private GameObject pixelEffectPrefab;
    [Tooltip("Time between each spawn in seconds.")]
    [SerializeField] private float spawnInterval = 1.0f;
    [Tooltip("Number of enemies remaining alive in the current wave.")]
    [SerializeField] private int enemiesAlive;
    private int enemiesToSpawn;
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize singleton instance or destroy duplicates.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Starts a new wave by setting counters and spawning pixel effects.
    /// </summary>
    /// <param name="count">Total number of enemies in this wave.</param>
    public void StartWave(int count)
    {
        enemiesToSpawn = count;
        enemiesAlive = count;
        StartCoroutine(SpawnWave());
    }

    /// <summary>
    /// Registers an enemy instance to call back on death.
    /// </summary>
    /// <param name="enemy">The enemy instance to register.</param>
    public void RegisterInstance(Enemy enemy)
    {
        enemy.OnDeath += EnemyDied;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Coroutine that spawns pixel effect prefabs at random spawn points.
    /// </summary>
    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            // Select a random spawn point
            var spawnPoint = spawnTransformList[Random.Range(0, spawnTransformList.Count)];

            // Instantiate the pixel effect for charging visualization
            var fx = Instantiate(pixelEffectPrefab, spawnPoint.position, spawnPoint.rotation);
            fx.GetComponent<PixelEffectController>().Init(this);

            // Wait before spawning next effect
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Callback invoked when a registered enemy dies; tracks wave completion.
    /// </summary>
    private void EnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0)
        {
            GameManager.Instance.OnWaveComplete();
        }
    }
    #endregion
}
