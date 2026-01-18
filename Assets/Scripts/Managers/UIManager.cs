using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Player UI")]
    [SerializeField] private GameObject player;

    [Header("Health UI")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private Image healthFillImageDelayed;
    [SerializeField] private TextMeshProUGUI healthPercentageText;
    [SerializeField] private float delayedLerpSpeed = 2f;

    [Header("Dash UI")]
    [SerializeField] private Image dashFillImage;

    [Header("Ammo UI")]
    [SerializeField] private Image heatFillImage;
    [SerializeField] private Color coolColor = Color.cyan;
    [SerializeField] private Color hotColor = Color.red;

    private float cooldownTimer = 0f;
    private Player playerScript;
    private PlayerWeapon playerWeapon;
    private bool canUpdate = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        if (player != null)
        {
            playerScript = player.GetComponent<Player>();
            playerWeapon = player.GetComponent<PlayerWeapon>();
            canUpdate = true;
        }
    }

    private void Update()
    {
        if (!canUpdate) return;

        UpdateWeaponUI();

        // Health
        float targetFill = playerScript.Health / (float)playerScript.MaxHealth;

        healthFillImage.fillAmount = targetFill;

        if (healthFillImageDelayed.fillAmount > targetFill)
        {
            healthFillImageDelayed.fillAmount =
                Mathf.Lerp(healthFillImageDelayed.fillAmount, targetFill, Time.deltaTime * delayedLerpSpeed);
        }
        else
        {
            healthFillImageDelayed.fillAmount = targetFill;
        }

        healthPercentageText.text = playerScript.Health + "%";
    }

    private void UpdateWeaponUI()
    {
        float heatPercent = (float)playerWeapon.CurrentAmmo / playerWeapon.CurrentMaxAmmo;

        // NORMAL HEAT (not overheated)
        if (heatPercent > 0f && !playerWeapon.IsCoolingDown)
        {
            cooldownTimer = 0f;

            heatFillImage.gameObject.SetActive(true);

            // Smooth fill
            heatFillImage.fillAmount = Mathf.Lerp(
                heatFillImage.fillAmount,
                heatPercent,
                Time.deltaTime * 10f
            );

            heatFillImage.color = Color.Lerp(coolColor, hotColor, heatFillImage.fillAmount);
            return;
        }

        // OVERHEATED COOLDOWN
        if (playerWeapon.IsCoolingDown)
        {
            heatFillImage.gameObject.SetActive(true);

            cooldownTimer += Time.deltaTime;
            float cooldownPercent = 1f - (cooldownTimer / playerWeapon.CooldownTime);

            heatFillImage.fillAmount = Mathf.Clamp01(cooldownPercent);
            heatFillImage.color = Color.Lerp(coolColor, hotColor, heatFillImage.fillAmount);

            if (cooldownPercent <= 0f)
            {
                heatFillImage.gameObject.SetActive(false);
                cooldownTimer = 0f;
            }

            return;
        }

        // FULLY COOLED
        heatFillImage.gameObject.SetActive(false);
        heatFillImage.fillAmount = 0f;
        heatFillImage.color = coolColor;
        cooldownTimer = 0f;
    }
}