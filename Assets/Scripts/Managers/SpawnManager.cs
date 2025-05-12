using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Singleton
    public static SpawnManager Instance { get; private set; }
    #endregion

    [Header("Spawn Settings")]
    [Tooltip("Where pixel effects (and later enemies) can appear.")]
    [SerializeField] private List<Transform> spawnTransformList = new List<Transform>();

    [Tooltip("Spawn interval between pixel effects (sec).")]
    [SerializeField] private float spawnInterval = 1f;

    [Tooltip("All your different PixelEffectController prefabs (red/blue/green/etc).")]
    [SerializeField] private List<GameObject> pixelEffectPrefabs = new List<GameObject>();

    private Coroutine spawnRoutine;
    private List<Enemy> activeEnemies = new List<Enemy>();
    private int killsThisWave;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Called by GameManager with 1-based wave number.
    /// </summary>
    public void StartWave(int waveNumber)
    {
        // reset state
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        foreach (var e in activeEnemies) if (e != null) Destroy(e.gameObject);
        activeEnemies.Clear();
        killsThisWave = 0;

        // compute duration: wave 1 = 10–20s, wave 2 = 20–30s, etc.
        float minTime = waveNumber * 10f;
        float maxTime = waveNumber * 10f + 10f;
        float duration = Random.Range(minTime, maxTime);

        Debug.Log("Current wave duration: " + duration);

        spawnRoutine = StartCoroutine(SpawnWave(duration));
    }

    private IEnumerator SpawnWave(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // choose a random spawn point & effect prefab
            var spawnPt = spawnTransformList[Random.Range(0, spawnTransformList.Count)];
            var fxPrefab = pixelEffectPrefabs[Random.Range(0, pixelEffectPrefabs.Count)];

            // instantiate the effect (it'll call RegisterInstance when it spawns its enemy)
            var fx = Instantiate(fxPrefab, spawnPt.position, spawnPt.rotation);
            fx.GetComponent<PixelEffectController>().Init(this);

            // wait and advance timer
            yield return new WaitForSeconds(spawnInterval);
            elapsed += spawnInterval;
        }

        EndCurrentWave();
    }

    /// <summary>
    /// PixelEffectController calls this once its actual enemy prefab has been instantiated.
    /// </summary>
    public void RegisterInstance(Enemy enemy)
    {
        activeEnemies.Add(enemy);
        // capture in closure so we know which one died
        Enemy e = enemy;
        e.OnDeath += () => OnEnemyDeath(e);
    }

    private void OnEnemyDeath(Enemy e)
    {
        killsThisWave++;
        activeEnemies.Remove(e);
        // no GameManager.OnWaveComplete here—wave ends by time, not count
    }

    private void EndCurrentWave()
    {
        // stop spawning further effects
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);

        // destroy any survivors without firing their death callbacks
        foreach (var e in activeEnemies)
            if (e != null) Destroy(e.gameObject);
        activeEnemies.Clear();

        // pass stats back to the GameManager & trigger completion
        GameManager.Instance.LastWaveKills = killsThisWave;
        Debug.Log("Enemies Killed this wave: " + killsThisWave);
        GameManager.Instance.OnWaveComplete();
    }
}
