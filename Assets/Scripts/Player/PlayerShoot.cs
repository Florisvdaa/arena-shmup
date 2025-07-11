using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    private Transform firePoint;
    private float fireCooldown = 0f;
    private MMF_Player shootFeedback;
    private PlayerSettings playerSettings;

    private void Start()
    {
        playerSettings = GetComponent<PlayerSettings>();
        firePoint = playerSettings.FirePoint;
        shootFeedback = playerSettings.ShootFeedback;
    }

    private void Update()
    {
        if (TimeManager.Instance.IsPaused || !GameManager.Instance.GetCanPlayerMove())
            return;

        fireCooldown -= Time.deltaTime;

        if (Mouse.current.leftButton.isPressed && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = playerSettings.CurrentFireRate;
        }
    }

    private void Shoot()
    {
        var bullet = BulletPool.Instance.GetBullet();
        if (bullet == null) return;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);

        shootFeedback?.PlayFeedbacks();
    }
}
