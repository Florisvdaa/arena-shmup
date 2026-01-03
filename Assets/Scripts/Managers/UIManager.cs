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

    [Header("Dash UI")]
    [SerializeField] private Slider dashSlider;

    [Header("Ammo UI")]
    [SerializeField] private TextMeshProUGUI currentAmmoText;
    [SerializeField] private TextMeshProUGUI maxAmmoText;
    [SerializeField] private Slider reloadSlider;

    // Reloading
    private float reloadTimer = 0f;

    private Player playerScript;
    private PlayerWeapon playerWeapon;

    [SerializeField] private float delayedLerpSpeed = 2f;

    private bool canUpdate = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        if(player != null)
        {
            playerScript = player.GetComponent<Player>();
            playerWeapon = player.GetComponent<PlayerWeapon>();

            canUpdate = true;
        }
    }

    private void Update()
    {
        if (!canUpdate) return;

        // Ammo
        currentAmmoText.text = playerWeapon.CurrentAmmo.ToString();
        maxAmmoText.text = " / " + playerWeapon.CurrentMaxAmmo.ToString();

        // Reloading
        if (playerWeapon.IsReloading)
        {
            reloadSlider.gameObject.SetActive(true);

            // Increase timer
            reloadTimer += Time.deltaTime;

            // Update slider
            reloadSlider.value = reloadTimer / playerWeapon.ReloadTime;
        }
        else
        {
            reloadSlider.gameObject.SetActive(false);
            reloadTimer = 0f;
        }

        // Health
        float targetFill = playerScript.Health / (float)playerScript.MaxHealth;

        // Instant bar
        healthFillImage.fillAmount = targetFill;

        // Delayed Bar (Lerp)
        if (healthFillImageDelayed.fillAmount > targetFill)
        {
            healthFillImageDelayed.fillAmount = Mathf.Lerp(healthFillImageDelayed.fillAmount, targetFill, Time.deltaTime * delayedLerpSpeed);
        }
        else
        {
            // If healing, snap instantly
            healthFillImageDelayed.fillAmount = targetFill;
        }    

        healthPercentageText.text = playerScript.Health + "%";
    }
}
