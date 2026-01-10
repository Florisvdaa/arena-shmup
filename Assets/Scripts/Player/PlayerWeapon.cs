using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private Player player;
    [SerializeField] private Transform spawnPointRight;
    [SerializeField] private Transform spawnPointLeft;
    [SerializeField] private Transform spawnPointCenter;
    [SerializeField] private WeaponSO currentWeapon;

    // Debug
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int ammo;
    [SerializeField] private int damage;
    [SerializeField] private float reloadTime;
    [SerializeField] private float fireRate; // shots per second
    
    [SerializeField] private float criticalHitChance = 0.2f;
    [SerializeField] private int lastBulletIncrease = 5;

    [SerializeField] private bool isUsingMoreSpawnpoints = false;

    private bool useRightSpawnPoint;
    private float nextFireTime;
    private bool isReloading;

    private GameObject bullet;


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

            if(isUsingMoreSpawnpoints)
            {
                spawnPointCenter.gameObject.SetActive(false);
            }
            else
            {
                spawnPointLeft.gameObject.SetActive(false);
                spawnPointRight.gameObject.SetActive(false);
            }
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
            Logger.Instance.Log(Color.white, "Out of ammo" , this.gameObject, "Player Weapon");

            // Reload
            StartReload();

            return;
        }

        Shoot();

        ammo--;

        nextFireTime = Time.time + (1f / fireRate);

        if(ammo <= 0)
        {
            StartReload();
        }
    }

    private void Shoot()
    {
        if (isUsingMoreSpawnpoints)
        {
            Transform spawnPoint = useRightSpawnPoint ? spawnPointRight : spawnPointLeft;
            //GameObject bullet = ObjectPoolManager.SpawnObject(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
            bullet = ObjectPoolManager.SpawnObject(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Transform spawnPoint = spawnPointCenter;
            bullet = ObjectPoolManager.SpawnObject(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        

        if (bullet == null)
            return; /* out of ammo feedback */ 
        
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();

        // Critical hit chance
        float randValue = Random.value;
        if(randValue < criticalHitChance)
        {
            int newDamage = damage * 2;
            bulletBehavior.Initialize(newDamage);

            Debug.Log(newDamage);
        }
        else
        {
            int currentDamage = damage;

            // When Ammo is almost empty increase the damage of the bullets
            if (ammo <= 2)
            {
                currentDamage += lastBulletIncrease;
            }
            
            //Debug.Log($"{currentDamage}");
            Logger.Instance.Log(Color.white, $"Damage: {currentDamage}", this.gameObject, "Player Weapon");

            bulletBehavior.Initialize(currentDamage);
        }

        useRightSpawnPoint = !useRightSpawnPoint;
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
        Logger.Instance.Log(Color.white, "Reloading...", this.gameObject, "Player Weapon");
        yield return new WaitForSeconds(reloadTime);

        ammo = maxAmmo;
        isReloading = false;

        Logger.Instance.Log(Color.white, "Reload Complete", this.gameObject, "Player Weapon");
    }

    // UI References
    public int CurrentAmmo => ammo;
    public int CurrentMaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;
    public float ReloadTime => reloadTime;
}
