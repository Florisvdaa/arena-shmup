using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Canvas settings")]
    [SerializeField] private Canvas uiCanvas;

    [Header("Start Screen")]
    [SerializeField] private GameObject mainMenuParent;
    [SerializeField] private Button startButton;

    [Header("Game UI")]
    [SerializeField] private GameObject gameUI;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI killChainMultiplier;
    [SerializeField] private TextMeshProUGUI currentPlayerLevel;
    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [SerializeField] private Slider playerExpSlider;
    [SerializeField] private TextMeshProUGUI expValueText;

    [Header("Upgrade UI")]
    [SerializeField] private GameObject upgradeParent;
    [SerializeField] private Button readyButton;
    [SerializeField] private GameObject waveCompleteText;

    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Countdown")]
    [SerializeField] private GameObject countdownFloatingTextPrefab;
    [SerializeField] private RectTransform countdownSpawnPoint;
    //[SerializeField] private TextMeshProUGUI countdownText;

    [Header("Floating text settings")]
    [SerializeField] private GameObject uiFloatingTextPrefab;
    [SerializeField] private RectTransform floatingTextSpawnPoint;

    [Header("DEBUG VALUE READER")]
    [SerializeField] private PlayerSettings playerSettings;
    [SerializeField] private TextMeshProUGUI playerspeed;
    [SerializeField] private TextMeshProUGUI playershoot;
    [SerializeField] private TextMeshProUGUI playerhealth;
    [SerializeField] private TextMeshProUGUI expMultiplierText;
    [SerializeField] private TextMeshProUGUI currentExpText;
    [SerializeField] private TextMeshProUGUI currentKillChhain;
    [SerializeField] private TextMeshProUGUI currentLevel;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        ButtonSetup();
        SetMainMenu();
    }
    private void Start()
    {
        UISetUp();
    }
    private void Update()
    {
        scoreText.text = "Score: " + ScoreManager.Instance.GetCurrentScore();
        waveText.text = GameManager.Instance.GetCurrentWave().ToString();
        killChainMultiplier.text = KillChainManager.Instance.GetKillChainMultiplier().ToString() + "X";
        currentPlayerLevel.text = ProgressManager.Instance.GetCurrentLevel().ToString();
        
        playerHealthSlider.maxValue = PlayerSettings.Instance.CurrentMaxHealth;
        playerHealthSlider.value = PlayerSettings.Instance.CurrentHealth;
        playerHealthText.text = $"HP: {(int)PlayerSettings.Instance.CurrentHealth} / {(int)PlayerSettings.Instance.CurrentMaxHealth}";
        // EXP Slider update
        playerExpSlider.maxValue = ProgressManager.Instance.GetEXPTillNextLevel();
        playerExpSlider.value = ProgressManager.Instance.GetCurrentEXP();
        expValueText.text = $"EXP: {(int)ProgressManager.Instance.GetCurrentEXP()} / {(int)ProgressManager.Instance.GetEXPTillNextLevel()}";

        // DEBUG
        //playerspeed.text = playerSettings.CurrentMovementSpeed.ToString();
        //playershoot.text = playerSettings.CurrentFireRate.ToString();
        //playerhealth.text = playerSettings.CurrentHealth.ToString();
        //expMultiplierText.text = playerSettings.CurrentExpMultiplier.ToString();
        //currentExpText.text = ProgressManager.Instance.GetCurrentEXP().ToString();
        //currentKillChhain.text = KillChainManager.Instance.GetKillChainMultiplier().ToString();
        //currentLevel.text = ProgressManager.Instance.GetCurrentLevel().ToString();
    }

    private void SetMainMenu()
    {
        mainMenuParent.SetActive(true);
    }
    public void UnsetMainMenu()
    {
        mainMenuParent.SetActive(false);
    }
    public void SetGameUI()
    {
        gameUI.SetActive(true);
    }
    public void UnsetGameUI()
    {
        gameUI.SetActive(false);
    }
    #region Game Start UI
    private void StartGame()
    {
        GameManager.Instance.StartGame();
    }
    #endregion
    #region UI Setup
    private void UISetUp()
    {
        scoreText.text = "Score: " + ScoreManager.Instance.GetCurrentScore();
        waveText.text = GameManager.Instance.GetCurrentWave().ToString();
        killChainMultiplier.text = KillChainManager.Instance.GetKillChainMultiplier().ToString();
        currentPlayerLevel.text = ProgressManager.Instance.GetCurrentLevel().ToString();

        playerHealthSlider.maxValue = PlayerSettings.Instance.CurrentMaxHealth;
        playerHealthSlider.value = PlayerSettings.Instance.CurrentHealth;
        playerHealthText.text = $"{(int)PlayerSettings.Instance.CurrentHealth} / {(int)PlayerSettings.Instance.CurrentMaxHealth}";

        playerExpSlider.maxValue = ProgressManager.Instance.GetEXPTillNextLevel();
        playerExpSlider.value = ProgressManager.Instance.GetCurrentEXP();
        expValueText.text = $"EXP: {(int)ProgressManager.Instance.GetCurrentEXP()} / {(int)ProgressManager.Instance.GetEXPTillNextLevel()}";
    }
    #endregion
    #region Countdown
    public void ShowCountdownText(string message)
    {
        //countdownText.text = message;
        //countdownText.gameObject.SetActive(true);
        GameObject instance = Instantiate(countdownFloatingTextPrefab, countdownSpawnPoint);
        RectTransform instanceRect = instance.GetComponent<RectTransform>();

        TMP_Text text = instance.GetComponentInChildren<TMP_Text>();
        if (text != null) text.text = message;

        MMF_Player player = instance.GetComponent<MMF_Player>();
        if (player != null) player.PlayFeedbacks();
    }
    public void HideCountdownText()
    {
        //countdownSpawnPoint.gameObject.SetActive(false);
        foreach (Transform child in countdownSpawnPoint)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion
    #region Wave Complete
    public void ShowWaveComplete()
    {
        waveCompleteText.SetActive(true);
        StartCoroutine(HideWaveComplete());
    }
    private IEnumerator HideWaveComplete()
    {
        yield return new WaitForSeconds(2f);
        waveCompleteText.SetActive(false);
    }
    public void ShowUpgradeUI()
    {
        upgradeParent.SetActive(true);
    }
    public void HideUpgradeUI()
    {
        upgradeParent.SetActive(false);
    }
    #endregion
    #region Floating text
    public void ShowFloatingText(string message)
    {
        GameObject instance = Instantiate(uiFloatingTextPrefab, floatingTextSpawnPoint.parent); // parent under canvas
        RectTransform instanceRect = instance.GetComponent<RectTransform>();

        // Match spawn point position
        instanceRect.anchorMin = floatingTextSpawnPoint.anchorMin;
        instanceRect.anchorMax = floatingTextSpawnPoint.anchorMax;
        instanceRect.pivot = floatingTextSpawnPoint.pivot;
        instanceRect.anchoredPosition = floatingTextSpawnPoint.anchoredPosition;

        var text = instance.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.text = message;
        }

        var mmfPlayer = instance.GetComponent<MMF_Player>();
        if (mmfPlayer != null)
        {
            mmfPlayer.PlayFeedbacks();
        }

        Destroy(instance, 2f); // Auto-destroy after animation
    }
    #endregion
    #region Button setup
    private void ButtonSetup()
    {
        startButton.onClick.AddListener(() => { StartGame(); });
        readyButton.onClick.AddListener(() => { GameManager.Instance.PlayerReadyForNextWave(); });
    }
    #endregion
    #region references
    public Canvas GetUICanvas() => uiCanvas;
    #endregion
}
