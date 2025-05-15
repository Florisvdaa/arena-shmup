using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnTransformList = new List<Transform>();
    [SerializeField] private List<GameObject> pixelEffectPrefabs = new List<GameObject>();

    [Header("Wave Scaling")]
    [SerializeField] private float baseSpawnInterval = 5f;
    [SerializeField] private float spawnDecreasePerRound = 0.1f;
    [SerializeField] private float minSpawnInterval = 0.5f;
    [SerializeField] private float roundDuration = 90f;

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

    public void StartWave(int waveNumber)
    {
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        foreach (var e in activeEnemies) if (e != null) Destroy(e.gameObject);
        activeEnemies.Clear();
        killsThisWave = 0;

        // Handle break rounds
        if (waveNumber % 5 == 0)
        {
            GameManager.Instance.OnBreakRound(waveNumber);
            return;
        }

        float spawnInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval - (waveNumber * spawnDecreasePerRound));
        spawnRoutine = StartCoroutine(SpawnWave(roundDuration, spawnInterval));
    }

    private IEnumerator SpawnWave(float duration, float interval)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            var spawnPt = spawnTransformList[Random.Range(0, spawnTransformList.Count)];
            var fxPrefab = pixelEffectPrefabs[Random.Range(0, pixelEffectPrefabs.Count)];

            var fx = Instantiate(fxPrefab, spawnPt.position, spawnPt.rotation);
            fx.GetComponent<PixelEffectController>().Init(this);

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        EndCurrentWave();
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
