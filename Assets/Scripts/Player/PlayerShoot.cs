using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shoot settings")]
    [SerializeField] private Transform firePoint; // Where bullets spawn from (e.g., a barrel tip)
    [SerializeField] private float fireRate = 0.2f; // Time between shots

    private float fireCooldown = 0f;

    [SerializeField] private MMF_Player shootFeedback; // Drag the MMF_Player here in Inspector
    private void Update()
    {
        fireCooldown -= Time.deltaTime;

        if (Mouse.current.leftButton.isPressed && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = fireRate;
        }
    }

    void Shoot()
    {
        GameObject bullet = BulletPool.Instance.GetBullet();
        if (bullet == null) return;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);

        shootFeedback?.PlayFeedbacks();
    }
}
