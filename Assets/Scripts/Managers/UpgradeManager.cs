using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button upgradeButton1;
    [SerializeField] private Button upgradeButton2;
    [SerializeField] private Button upgradeButton3;

    [Header("Stat Display References")]
    [SerializeField] private TextMeshProUGUI textFireRate;
    [SerializeField] private TextMeshProUGUI textFireDamage;
    [SerializeField] private TextMeshProUGUI textMovementSpeed;
    [SerializeField] private TextMeshProUGUI textDashDuration;
    [SerializeField] private TextMeshProUGUI textHealth;
    [SerializeField] private TextMeshProUGUI textDefense;


    private List<Upgrade> speedUpgrades = new List<Upgrade>();
    private List<Upgrade> firepowerUpgrades = new List<Upgrade>();
    private List<Upgrade> defenseUpgrades = new List<Upgrade>();

    private List<Upgrade> currentChoices = new List<Upgrade>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        upgradePanel.SetActive(false);

        // Hook up buttons
        upgradeButton1.onClick.AddListener(() => ApplyUpgrade(0));
        upgradeButton2.onClick.AddListener(() => ApplyUpgrade(1));
        upgradeButton3.onClick.AddListener(() => ApplyUpgrade(2));

        SetupUpgrades();
    }

    private void SetupUpgrades()
    {
        // SPEED CATEGORY
        speedUpgrades.Add(new Upgrade("Increase Speed", UpgradeCategory.Speed, () => PlayerSettings.Instance.IncreaseSpeed(2f)));
        speedUpgrades.Add(new Upgrade("Longer Dash", UpgradeCategory.Speed, () => PlayerSettings.Instance.IncreaseDashLength(0.1f)));

        // FIREPOWER CATEGORY
        firepowerUpgrades.Add(new Upgrade("Faster Fire Rate", UpgradeCategory.Firepower, () => PlayerSettings.Instance.IncreaseFireRate(0.05f)));
        firepowerUpgrades.Add(new Upgrade("Stronger Bullets", UpgradeCategory.Firepower, () => PlayerSettings.Instance.IncreaseFireDamage(1f)));

        // DEFENSE CATEGORY
        defenseUpgrades.Add(new Upgrade("More Health", UpgradeCategory.Defense, () => PlayerSettings.Instance.IncreaseHealth(5f)));
        defenseUpgrades.Add(new Upgrade("Reduce Damage Taken", UpgradeCategory.Defense, () => PlayerSettings.Instance.IncreaseDefense(0.1f)));
    }

    public void ShowUpgradeOptions()
    {
        upgradePanel.SetActive(true);
        currentChoices.Clear();

        currentChoices.Add(GetRandomUpgradeFromCategory(speedUpgrades));
        currentChoices.Add(GetRandomUpgradeFromCategory(firepowerUpgrades));
        currentChoices.Add(GetRandomUpgradeFromCategory(defenseUpgrades));

        upgradeButton1.GetComponentInChildren<TextMeshProUGUI>().text = currentChoices[0].label;
        upgradeButton2.GetComponentInChildren<TextMeshProUGUI>().text = currentChoices[1].label;
        upgradeButton3.GetComponentInChildren<TextMeshProUGUI>().text = currentChoices[2].label;

        UpdateStatDisplay();
    }
    private void UpdateStatDisplay()
    {
        var ps = PlayerSettings.Instance;
        textFireRate.text = $"Fire Rate: {ps.CurrentFireRate:F2}";
        textFireDamage.text = $"Fire Damage: {ps.CurrentFireDamage:F1}";
        textMovementSpeed.text = $"Movement Speed: {ps.CurrentMovementSpeed:F1}";
        textDashDuration.text = $"Dash Duration: {ps.DashDuration:F1}";
        textHealth.text = $"Health: {ps.CurrentHealth:F0}/{ps.CurrentMaxHealth:F0}";
        textDefense.text = $"Defense: {ps.CurrentDefenseMultiplier:P0}";
    }

    private Upgrade GetRandomUpgradeFromCategory(List<Upgrade> categoryList)
    {
        return categoryList[Random.Range(0, categoryList.Count)];
    }

    private void ApplyUpgrade(int index)
    {
        if (index >= 0 && index < currentChoices.Count)
        {
            currentChoices[index].applyEffect.Invoke();
        }

        StartCoroutine(StartNewRound());
    }

    private IEnumerator StartNewRound()
    {
        UpdateStatDisplay();

        yield return new WaitForSeconds(1);

        upgradePanel.SetActive(false);
        GameManager.Instance.PlayerChoseUpgrade();
    }
}
