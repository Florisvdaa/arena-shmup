using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnTransformList = new List<Transform>();
    [SerializeField] private List<GameObject> pixelEffectPrefabs = new List<GameObject>();
    [SerializeField] private List<Transform> bossSpawnPoints = new List<Transform>();

    [Header("Wave Scaling")]
    [SerializeField] private float baseSpawnInterval = 5f;
    [SerializeField] private float spawnDecreasePerRound = 0.1f;
    [SerializeField] private float minSpawnInterval = 0.5f;
    [SerializeField] private float roundDuration = 90f;
    [SerializeField] private int baseEnemiesPerWave = 10;
    [SerializeField] private int enemiesIncreasePerWave = 2;

    [Header("Enemy Scaling")]
    [SerializeField] private float baseEnemyHealth = 1f;
    [SerializeField] private float healthIncreasePerWave = 1.5f;
    [SerializeField] private float baseEnemyDamage = 1f;
    [SerializeField] private float damageIncreasePerWave = 0.5f;


    [Header("Boss Settings")]
    [SerializeField] private GameObject miniBossPrefab;

    private Coroutine spawnRoutine;
    private List<Enemy> activeEnemies = new List<Enemy>();
    private int killsThisWave;
    private int currentWave;

    // Public accessors for scaling values
    public float BaseEnemyHealth => baseEnemyHealth;
    public float HealthIncreasePerWave => healthIncreasePerWave;
    public float BaseEnemyDamage => baseEnemyDamage;
    public float DamageIncreasePerWave => damageIncreasePerWave;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartWave(int waveNumber)
    {
        //if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        //foreach (var e in activeEnemies) if (e != null) Destroy(e.gameObject);
        //activeEnemies.Clear();
        //killsThisWave = 0;
        //currentWave = waveNumber;

        //// Handle break rounds
        //if (waveNumber % 5 == 0)
        //{
        //    SpawnMiniBoss(waveNumber);
        //    GameManager.Instance.OnBreakRound(waveNumber); // optional: trigger upgrade room etc.
        //    return;
        //}

        //float spawnInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval - (waveNumber * spawnDecreasePerRound));
        //spawnRoutine = StartCoroutine(SpawnWave(roundDuration, spawnInterval));

        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        foreach (var e in activeEnemies)
            if (e != null) Destroy(e.gameObject);
        activeEnemies.Clear();

        killsThisWave = 0;
        currentWave = waveNumber;

        int enemiesToSpawn = baseEnemiesPerWave + (waveNumber * enemiesIncreasePerWave);
        bool shouldSpawnBoss = waveNumber >= 5;

        spawnRoutine = StartCoroutine(SpawnEnemies(enemiesToSpawn, shouldSpawnBoss));
    }
    private IEnumerator SpawnEnemies(int totalEnemies, bool includeBoss)
    {
        int spawnedEnemies = 0;
        float spawnInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval - (currentWave * spawnDecreasePerRound));

        if (includeBoss)
        {
            SpawnMiniBoss(currentWave); // includes pixel effect spawn
            yield return new WaitForSeconds(1f); // optional: spacing before normal enemies
        }

        while (spawnedEnemies < totalEnemies)
        {
            var spawnPt = spawnTransformList[Random.Range(0, spawnTransformList.Count)];
            var fxPrefab = pixelEffectPrefabs[Random.Range(0, pixelEffectPrefabs.Count)];

            var fx = Instantiate(fxPrefab, spawnPt.position, spawnPt.rotation);
            fx.GetComponent<PixelEffectController>().Init(this, currentWave);

            spawnedEnemies++;
            yield return new WaitForSeconds(spawnInterval);
        }

        spawnRoutine = null;
    }


    private IEnumerator SpawnWave(float duration, float interval)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            var spawnPt = spawnTransformList[Random.Range(0, spawnTransformList.Count)];
            var fxPrefab = pixelEffectPrefabs[Random.Range(0, pixelEffectPrefabs.Count)];

            var fx = Instantiate(fxPrefab, spawnPt.position, spawnPt.rotation);
            fx.GetComponent<PixelEffectController>().Init(this, currentWave);

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        EndCurrentWave();
    }

    private void SpawnMiniBoss(int waveNumber)
    {
        var spawnPt = bossSpawnPoints[Random.Range(0, bossSpawnPoints.Count)];
        var fxPrefab = pixelEffectPrefabs[Random.Range(0, pixelEffectPrefabs.Count)];

        var fx = Instantiate(fxPrefab, spawnPt.position, spawnPt.rotation);
        fx.GetComponent<PixelEffectController>().Init(this, waveNumber);
    }


    public void RegisterInstance(Enemy enemy)
    {
        activeEnemies.Add(enemy);
        Enemy e = enemy;
        e.OnDeath += () => OnEnemyDeath(e);
    }

    private void OnEnemyDeath(Enemy e)
    {
        killsThisWave++;
        activeEnemies.Remove(e);

        if (activeEnemies.Count == 0 && spawnRoutine == null)
        {
            EndCurrentWave();
        }
    }


    private void EndCurrentWave()
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);

        foreach (var e in activeEnemies)
            if (e != null) Destroy(e.gameObject);
        activeEnemies.Clear();

        GameManager.Instance.LastWaveKills = killsThisWave;
        GameManager.Instance.OnWaveComplete();
    }
}
