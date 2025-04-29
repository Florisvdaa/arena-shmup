using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [Header("Shoot Settings")]
    private Transform firePoint; // Where bullets spawn 
    private float fireCooldown = 0f;
    
    private MMF_Player shootFeedback; // Drag the MMF_Player here in Inspector
    private PlayerSettings playerSettings;

    private void Start()
    {
        playerSettings = GetComponent<PlayerSettings>();

        firePoint = playerSettings.FirePoint;
        shootFeedback = playerSettings.ShootFeedback;
    }
    private void Update()
    {
        if (GameManager.Instance.GetCanPlayerMove())
        {
            fireCooldown -= Time.deltaTime;

            if (Mouse.current.leftButton.isPressed && fireCooldown <= 0f)
            {
                Shoot();
                fireCooldown = playerSettings.CurrentFireRate;
            }
        }
    }
    private void Shoot()
    {
        GameObject bullet = BulletPool.Instance.GetBullet();
        if (bullet == null) return;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);

        shootFeedback?.PlayFeedbacks();
    }
}
