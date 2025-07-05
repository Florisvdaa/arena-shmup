using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button upgradeButton1;
    [SerializeField] private Button upgradeButton2;
    [SerializeField] private Button upgradeButton3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        upgradePanel.SetActive(false);

        upgradeButton1.onClick.AddListener(() => ApplyUpgrade(0));
        upgradeButton2.onClick.AddListener(() => ApplyUpgrade(1));
        upgradeButton3.onClick.AddListener(() => ApplyUpgrade(2));
    }

    public void ShowUpgradeOptions()
    {
        upgradePanel.SetActive(true);

        // TODO: Randomly choose upgrades or define them
        upgradeButton1.GetComponentInChildren<TextMeshProUGUI>().text = "Increase Speed";
        upgradeButton2.GetComponentInChildren<TextMeshProUGUI>().text = "Faster Fire Rate";
        upgradeButton3.GetComponentInChildren<TextMeshProUGUI>().text = "More Health";
    }

    private void ApplyUpgrade(int index)
    {
        switch (index)
        {
            case 0: PlayerSettings.Instance.IncreaseSpeed(2f); break;
            case 1: PlayerSettings.Instance.IncreaseFireRate(0.05f); break;
            case 2: PlayerSettings.Instance.IncreaseHealth(5f); break;
        }

        upgradePanel.SetActive(false);
        GameManager.Instance.PlayerChoseUpgrade();
    }
}
