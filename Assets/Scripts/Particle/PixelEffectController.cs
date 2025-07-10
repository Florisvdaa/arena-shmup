using MoreMountains.Feedbacks;
using UnityEngine;

public class PixelEffectController : MonoBehaviour
{
    [SerializeField] private MMF_Player chargeFeedback;
    [SerializeField] private GameObject enemyPrefab;

    private SpawnManager spawnManager;
    private PixelChargeInEffect charger;
    private int waveNumber;

    public void Init(SpawnManager manager, int wave)
    {
        spawnManager = manager;
        waveNumber = wave;
        charger = GetComponentInChildren<PixelChargeInEffect>();
        PlayChargeEffect();
    }

    public void PlayChargeEffect()
    {
        chargeFeedback?.PlayFeedbacks();
        charger?.StartCharging(this);
    }

    public void TriggerEnemySpawn()
    {
        var go = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        var enemy = go.GetComponent<Enemy>();

        float scaledHealth = spawnManager != null
            ? spawnManager.BaseEnemyHealth + (waveNumber * spawnManager.HealthIncreasePerWave)
            : 10f;
        float scaledDamage = spawnManager != null
            ? spawnManager.BaseEnemyDamage + (waveNumber * spawnManager.DamageIncreasePerWave)
            : 1f;

        enemy.Initialize(scaledHealth, scaledDamage);
        spawnManager.RegisterInstance(enemy);

        Destroy(gameObject);
    }
}
