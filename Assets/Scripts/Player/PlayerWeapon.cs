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
    [SerializeField] private float ammo;
    [SerializeField] private int damage;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float fireRate; // shots per second
    [SerializeField] private float passiveCooldownRate = 5f; // Ammo per second
    private float timeSinceLastFire = 0f;
    [SerializeField] private float passiveCooldownDelay = 1f; // Seconds before cooldown

    [SerializeField] private float criticalHitChance = 0.2f;
    [SerializeField] private int lastBulletIncrease = 5;

    [SerializeField] private bool isUsingMoreSpawnpoints = false;

    private bool useRightSpawnPoint;
    private float nextFireTime;
    private bool isCoolingDown;
    private GameObject bullet;

    private void Start()
    {
        player = GetComponent<Player>();

        if(currentWeapon != null )
        {
            maxAmmo = currentWeapon.overheatCap;
            ammo = 0;
            damage = currentWeapon.damage;
            cooldownTime = currentWeapon.cooldownTime;
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
        if (player == null || !player.PlayerCanMove)
            return;

        if (player.IsFireHolding)
        {
            timeSinceLastFire = 0f;
            TryShoot();
        }
        else
        {
            timeSinceLastFire += Time.deltaTime;

            if (timeSinceLastFire >= passiveCooldownDelay)
            {
                // Player is NOT firing for cooldown delay -> start cooldown
                PassiveCooldown();
            }
        }
    }

    private void TryShoot()
    {
        if (isCoolingDown)
            return;

        if (Time.time < nextFireTime)
            return;

        // Overheat
        if (ammo >= maxAmmo)
        {
            StartCooldown(true);
            return;
        }

        Shoot();

        ammo += 1f;
        //ammo += Time.deltaTime * fireRate; // Test

        nextFireTime = Time.time + (1f / fireRate);
    }

    private void Shoot()
    {
        if (isUsingMoreSpawnpoints)
        {
            Transform spawnPoint = useRightSpawnPoint ? spawnPointRight : spawnPointLeft;
            bullet = ObjectPoolManager.SpawnObject(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            Transform spawnPoint = spawnPointCenter;
            bullet = ObjectPoolManager.SpawnObject(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        if (bullet == null)
            return; /* weapon overheat feedback */ 
        
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
            if (ammo >= (maxAmmo - 2))
            {
                currentDamage += lastBulletIncrease;
            }
            
            //Debug.Log($"{currentDamage}");
            Logger.Instance.Log(Color.white, $"Damage: {currentDamage}", this.gameObject, "Player Weapon");

            bulletBehavior.Initialize(currentDamage);
        }

        useRightSpawnPoint = !useRightSpawnPoint;
    }
    public void StartCooldown(bool overheated)
    {
        if (isCoolingDown)
            return;

        isCoolingDown = true;
        StartCoroutine(Cooldown(overheated));
    }

    /// <summary>
    /// cools down the weapon smoothly when player stops firing
    /// </summary>
    private void PassiveCooldown()
    {
        if (isCoolingDown) return;

        if (ammo > 0f)
        {
            ammo -= passiveCooldownRate * Time.deltaTime;
            ammo = Mathf.Clamp(ammo, 0f, maxAmmo);
        }

    }

    private IEnumerator Cooldown(bool overheated)
    {

        float time = overheated ? cooldownTime : cooldownTime * 0.25f;

        Logger.Instance.Log(Color.white, overheated ? "OVERHEATED!" : "Cooling...", this.gameObject, "Player Weapon");

        yield return new WaitForSeconds(time);

        ammo = 0;
        isCoolingDown = false;

        Logger.Instance.Log(Color.white, "Cooldown complete", this.gameObject, "Player Weapon");

    }

    // UI References
    public int CurrentAmmo => Mathf.RoundToInt(ammo);
    public int CurrentMaxAmmo => maxAmmo;
    public bool IsCoolingDown => isCoolingDown;
    public float CooldownTime => cooldownTime;
    public float PassiveCooldownDuration => (float)ammo / passiveCooldownRate;
    public float PassiveCooldownTimePerHeat => 1f / passiveCooldownRate;
    public float PassiveCooldownDurationFromHeat(float heatPercent)
    {
        return heatPercent * (1f / passiveCooldownRate);
    }
}
