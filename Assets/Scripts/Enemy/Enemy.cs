using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private Transform enemyVisual;
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            other.GetComponent<Player>().TakeDamage(damage);
            Destroy(this.gameObject, 0.5f);
        }
    }
}
