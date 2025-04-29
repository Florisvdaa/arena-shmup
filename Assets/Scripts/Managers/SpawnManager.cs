using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Spawn Settings")]
    [SerializeField] private List<Transform> spawnTransformList = new List<Transform>();
    [SerializeField] private GameObject enemyPrefab; // Assign your enemy prefab in the Inspector
    [SerializeField] private float spawnInterval = 1.0f;

    private int enemiesToSpawn;
    private int enemiesAlive;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public void StartWave(int enemyCount)
    {
        enemiesToSpawn = enemyCount;
        enemiesAlive = enemyCount;
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < enemiesToSpawn; i++) 
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        } 
    
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
    }
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (spawnTransformList.Count == 0 || enemyPrefab == null)
        {
            Debug.LogWarning("Spawn list is empty or enemyPrefab not assigned.");
            return;
        }

        Transform randomSpawn = spawnTransformList[Random.Range(0, spawnTransformList.Count)];
        GameObject enemy = Instantiate(enemyPrefab, randomSpawn.position, randomSpawn.rotation);
        enemy.GetComponent<Enemy>().OnDeath += EnemyDied; // hook up death callback
    }
    private void EnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0)
        {
            GameManager.Instance.OnWaveComplete();
        }
    }
}
