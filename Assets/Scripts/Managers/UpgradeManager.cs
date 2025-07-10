using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button upgradeButton1;
    [SerializeField] private Button upgradeButton2;
    [SerializeField] private Button upgradeButton3;

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
        speedUpgrades.Add(new Upgrade("Longer Dash", UpgradeCategory.Speed, () => PlayerSettings.Instance.IncreaseDashLength(1f)));

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

        upgradePanel.SetActive(false);
        GameManager.Instance.PlayerChoseUpgrade();
    }
}
