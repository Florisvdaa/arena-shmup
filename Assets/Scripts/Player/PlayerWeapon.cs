using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private Player player;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WeaponSO currentWeapon;

    // Debug
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int ammo;
    [SerializeField] private int damage;
    [SerializeField] private float reloadTime;
    [SerializeField] private float fireRate; // shots per second

    private float nextFireTime;

    private bool isReloading;

    private void Start()
    {
        player = GetComponent<Player>();

        if(currentWeapon != null )
        {
            maxAmmo = currentWeapon.maxAmmo;
            ammo = currentWeapon.maxAmmo;
            damage = currentWeapon.damage;
            reloadTime = currentWeapon.reloadTime;
            fireRate = currentWeapon.fireRate;
            bulletPrefab = currentWeapon.bulletPrefab;
        }

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
        if (isReloading)
            return;

        if (Time.time < nextFireTime)
            return;

        if (ammo <= 0)
        {
            Debug.Log("Out of ammo");

            // Reload
            StartReload();

            return;
        }

        Shoot();

        ammo--;
        nextFireTime = Time.time + (1f / fireRate);
    }

    private void Shoot()
    {

        GameObject bullet = ObjectPoolManager.SpawnObject(bulletPrefab, spawnPoint.position, spawnPoint.rotation);

        if (bullet == null)
            return; /* out of ammo feedback */ 
        
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        bulletBehavior.Initialize(damage); // expand this with the WeaponSO -> set the data in the Initialize in the bullet instead in here
    }
    public void StartReload()
    {
        if (isReloading || ammo == maxAmmo)
            return;

        isReloading = true;
        StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);

        ammo = maxAmmo;
        isReloading = false;

        Debug.Log("Reload complete");
    }

    // UI References
    public int CurrrentAmmo => ammo;
    public int CurrentMaxAmmo => maxAmmo;
}
