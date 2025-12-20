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
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider dashSlider;
    [SerializeField] private TextMeshProUGUI ammoText;

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
        if(player != null)
        {
            playerScript = player.GetComponent<Player>();
            playerWeapon = player.GetComponent<PlayerWeapon>();

            healthSlider.maxValue = playerScript.Health;
            healthSlider.value = playerScript.Health;

            canUpdate = true;
        }
    }

    private void Update()
    {
        if (canUpdate)
        {
            ammoText.text = playerWeapon.CurrrentAmmo.ToString() + " / " + playerWeapon.CurrentMaxAmmo.ToString();

            healthSlider.value = playerScript.Health;
        }   
    }
}
