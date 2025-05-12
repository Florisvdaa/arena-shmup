using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Manages available power-ups, random selection, and activation.
 */
public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [Tooltip("All PowerUpDefinition assets in your project")]
    [SerializeField] private List<PowerUpDefinition> allPowerUps;

    private List<PowerUpDefinition> nextWaveSelections = new List<PowerUpDefinition>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Call at end of wave to prepare random choices.
    /// </summary>
    public List<PowerUpDefinition> GetRandomChoices(int count)
    {
        nextWaveSelections.Clear();
        var pool = new List<PowerUpDefinition>(allPowerUps);
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int idx = Random.Range(0, pool.Count);
            nextWaveSelections.Add(pool[idx]);
            pool.RemoveAt(idx);
        }
        return nextWaveSelections;
    }

    /// <summary>
    /// Activate a chosen power-up at wave start.
    /// </summary>
    public void Activate(PowerUpDefinition def)
    {
        // Stop any existing coroutines for same type?
        switch (def.type)
        {
            case PowerUpType.Shotgun:
                StartCoroutine(PlayerSettings.Instance.FireRateBoostCoroutine(-0.15f, def.duration));
                // Replace weapon, or adjust shot count, etc.
                break;
            case PowerUpType.SlowMotion:
                TimeManager.Instance.EnterSlowMotion();
                StartCoroutine(DisableSlowMotionAfter(def.duration));
                break;
                // ... handle other cases
        }
    }

    private IEnumerator<WaitForSeconds> DisableSlowMotionAfter(float t)
    {
        yield return new WaitForSeconds(t);
        TimeManager.Instance.ExitSlowMotion();
    }
}
