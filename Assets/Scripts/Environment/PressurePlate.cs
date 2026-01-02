using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{

    [SerializeField] private bool isActive = false;

    private Laser laser;

    private void Awake()
    {
        laser = GetComponentInChildren<Laser>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isActive && other.gameObject.CompareTag("Player"))
        {
            if (laser != null)
                laser.ActivateLaser();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if (laser != null)
                laser.DeactivateLaser();
        }
    }
}
