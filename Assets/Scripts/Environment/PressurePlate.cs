using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private bool isActive = false;
    [SerializeField] private List<MonoBehaviour> connectedHazards;

    private List<IHazard> hazards = new List<IHazard>();
    private bool isResetting = false;

    private void Awake()
    {
        foreach (var mb in connectedHazards)
        {
            if (mb is IHazard hazard)
                hazards.Add(hazard);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive && other.CompareTag("Player"))
        {
            isActive = true;

            foreach (var hazard in hazards)
                hazard.Activate();

            StartResetRoutineOnce();
        }
    }

    private void Update()
    {
        if (isActive && AllHazardsInactive())
        {
            StartResetRoutineOnce();
        }
    }

    private bool AllHazardsInactive()
    {
        foreach (IHazard hazard in hazards)
        {
            if (hazard.IsActive)
                return false;
        }

        return true;
    }

    private void StartResetRoutineOnce()
    {
        if (!isResetting)
            StartCoroutine(ResetRoutine());
    }

    private IEnumerator ResetRoutine()
    {
        isResetting = true;
        Debug.Log("Pressure plate is resetting...");

        yield return new WaitForSeconds(5f);

        isActive = false;
        isResetting = false;

        Debug.Log("Pressure plate has been reset");
    }
}