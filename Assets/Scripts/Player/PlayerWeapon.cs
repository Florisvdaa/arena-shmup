using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private Player player;

    // Debug
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int ammo = 30;
    [SerializeField] private int damage = 10;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private float fireRate = 5f; // shots per second

    private float nextFireTime;

    private void Start()
    {
        player = GetComponent<Player>();

        ObjectPoolManager.PrewarmPool(bulletPrefab, maxAmmo);
    }

    private void Update()
    {
        if (player != null && player.IsFireHolding)
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (Time.time < nextFireTime)
            return;

        if (ammo <= 0)
        {
            Debug.Log("Out of ammo");
            return;
        }

        Shoot();

        ammo--;
        nextFireTime = Time.time + (1f / fireRate);
    }

    private void Shoot()
    {
        Debug.Log("Shoot");
        // objectPoolManager.SpawnObject(bulletPrefab, spawnPoint.position, spawnPoint.transform.rotation);

        GameObject bullet = ObjectPoolManager.SpawnObject(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        if (bullet == null) { /* out of ammo feedback */ }

        // Raycast / spawn bullet / apply damage here
    }
}
