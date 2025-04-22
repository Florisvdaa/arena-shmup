using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private Transform enemyVisual;
    [SerializeField] private int damage = 1;
    [SerializeField] private int health = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            other.GetComponent<Player>().TakeDamage(damage);
            Destroy(this.gameObject, 0.5f);
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0)
        {
            // referance to the score system, add score for example
            Destroy(this.gameObject);
        }
    }
}
