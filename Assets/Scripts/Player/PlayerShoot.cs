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
    private float currentHeat = 0f;
    private bool isOverheated = false;

    private MMF_Player shootFeedback;
    private MMF_Player overheatFeedback;
    private PlayerSettings playerSettings;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        playerSettings = GetComponent<PlayerSettings>();
        firePoint = playerSettings.FirePoint;
        shootFeedback = playerSettings.ShootFeedback;
        overheatFeedback = playerSettings.OverheatFeedback; // Add this in PlayerSettings
    }

    private void Update()
    {
        if (TimeManager.Instance.IsPaused || !GameManager.Instance.GetCanPlayerMove())
            return;

        fireCooldown -= Time.deltaTime;

        HandleCooling();

        if (Mouse.current.leftButton.isPressed && fireCooldown <= 0f && !isOverheated)
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

        AddHeat(playerSettings.HeatPerShot);
    }

    private void AddHeat(float amount)
    {
        currentHeat += amount;

        if (currentHeat >= playerSettings.OverheatThreshold)
        {
            currentHeat = playerSettings.OverheatThreshold;
            isOverheated = true;
            overheatFeedback?.PlayFeedbacks();
        }
    }

    private void HandleCooling()
    {
        if (!Mouse.current.leftButton.isPressed || isOverheated)
        {
            currentHeat -= playerSettings.CoolRate * Time.deltaTime;
            currentHeat = Mathf.Max(0f, currentHeat);

            if (isOverheated && currentHeat <= playerSettings.OverheatReleaseThreshold)
            {
                isOverheated = false;
            }
        }
    }
    #endregion
}
