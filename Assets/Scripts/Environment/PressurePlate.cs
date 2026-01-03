using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private bool isActive = false;
    [SerializeField] private List<MonoBehaviour> connectedHazards;

    private List<IHazard> hazards = new List<IHazard>();

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
            foreach (var hazard in hazards)
                hazard.Activate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (var hazard in hazards)
                hazard.Deactivate();
        }
    }
}
