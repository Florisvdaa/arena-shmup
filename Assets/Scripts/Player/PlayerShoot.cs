using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles firing bullets based on input and fire rate.
/// </summary>
public class PlayerShoot : MonoBehaviour
{
    #region Private Fields
    private Transform firePoint;
    private float fireCooldown = 0f;
    private MMF_Player shootFeedback;
    private PlayerSettings playerSettings;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        playerSettings = GetComponent<PlayerSettings>();
        firePoint = playerSettings.FirePoint;
        shootFeedback = playerSettings.ShootFeedback;
    }

    private void Update()
    {
        if (TimeManager.Instance.IsPaused) return;
        if (!GameManager.Instance.GetCanPlayerMove()) return;

        fireCooldown -= Time.deltaTime;
        if (Mouse.current.leftButton.isPressed && fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = playerSettings.CurrentFireRate;
        }
    }
    #endregion

    #region Private Methods
    private void Shoot()
    {
        var bullet = BulletPool.Instance.GetBullet();
        if (bullet == null) return;

        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.SetActive(true);

        shootFeedback?.PlayFeedbacks();
    }
    #endregion
}
