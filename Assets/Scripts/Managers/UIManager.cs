using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

/// <summary>
/// Handles all UI elements: menus, HUD updates, floating texts, and upgrade controls.
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Global access point for the UIManager instance.
    /// </summary>
    public static UIManager Instance { get; private set; }
    #endregion

    #region Inspector Fields
    [Header("Canvas Settings")]
    [Tooltip("Main UI canvas for all UI elements.")]
    [SerializeField] private Canvas uiCanvas;

    [Header("Canvas Animators")]
    [Tooltip("Animators that should update in unscaled time.")]
    [SerializeField] private Animator[] uiAnimators;

    [Header("Start Screen")]
    [Tooltip("Parent object containing main menu UI.")]
    [SerializeField] private GameObject mainMenuParent;
    [Tooltip("Button to start the game.")]
    [SerializeField] private Button startButton;

    [Header("Game HUD")]
    [Tooltip("Parent object for in-game HUD.")]
    [SerializeField] private GameObject gameUI;
    [Tooltip("Displays current wave number.")]
    [SerializeField] private TextMeshProUGUI waveText;
    [Tooltip("Displays current kill chain multiplier.")]
    [SerializeField] private TextMeshProUGUI killChainMultiplier;
    [Tooltip("Displays player's current level.")]
    [SerializeField] private TextMeshProUGUI currentPlayerLevel;
    [Tooltip("Slider for player health.")]
    [SerializeField] private Slider playerHealthSlider;
    [Tooltip("Text for player health value.")]
    [SerializeField] private TextMeshProUGUI playerHealthText;
    [Tooltip("Slider for player EXP.")]
    [SerializeField] private Slider playerExpSlider;
    [Tooltip("Text for EXP values.")]
    [SerializeField] private TextMeshProUGUI expValueText;

    [Header("Upgrade UI")]
    [Tooltip("Parent object for upgrade menu.")]
    [SerializeField] private GameObject upgradeParent;
    [Tooltip("Parent object for upgrade hold button and slider.")]
    [SerializeField] private GameObject upgradeHoldButtonParent;
    [Tooltip("Image used as upgrade hold progress.")]
    [SerializeField] private Image upgradeHoldImage;
    [Tooltip("Button to confirm upgrade.")]
    [SerializeField] private Button readyButton;
    [Tooltip("Text object shown when wave is complete.")]
    [SerializeField] private GameObject waveCompleteText;

    [Header("Score Display")]
    [Tooltip("Text showing current score.")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Countdown Floating Text")]
    [Tooltip("Prefab for countdown floating text.")]
    [SerializeField] private GameObject countdownFloatingTextPrefab;
    [Tooltip("Spawn point for countdown text (RectTransform).")]
    [SerializeField] private RectTransform countdownSpawnPoint;

    [Header("General Floating Text")]
    [Tooltip("Prefab for generic floating text (e.g. damage indicators).")]
    [SerializeField] private GameObject uiFloatingTextPrefab;
    [Tooltip("RectTransform that defines spawn position for floating text.")]
    [SerializeField] private RectTransform floatingTextSpawnPoint;

    [Header("DEBUG")]
    [SerializeField] private TextMeshProUGUI PUPText;
    [SerializeField] private TextMeshProUGUI PUPsToSpedText;
    [SerializeField] private TextMeshProUGUI skillPointText;

    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize singleton, animator update modes, and menu states.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure animators use unscaled delta time
        foreach (var anim in uiAnimators)
            anim.updateMode = AnimatorUpdateMode.UnscaledTime;

        ShowUpgradeControls(false);
        SetupButtons();
        SetMainMenu();
    }

    /// <summary>
    /// Subscribe to events and initialize HUD values.
    /// </summary>
    private void Start()
    {
        //ProgressManager.Instance.OnUpgradeAvailabilityChanged += ShowUpgradeControls;
        UpdateAllUI();
    }

    /// <summary>
    /// Update HUD every frame with current game values.
    /// </summary>
    private void Update()
    {
        scoreText.text = "Score: " + ScoreManager.Instance.GetCurrentScore();
        waveText.text = GameManager.Instance.GetCurrentWave().ToString();
        killChainMultiplier.text = KillChainManager.Instance.GetKillChainMultiplier() + "X";

        PUPText.text = ProgressManager.Instance.GetCurrentPUP().ToString();
        PUPsToSpedText.text = ProgressManager.Instance.GetAvailablePUP().ToString();
        skillPointText.text = UpgradeManager.Instance.GetCurrentSkillPoints().ToString();
    }
    #endregion

    #region Menu Toggles
    /// <summary>
    /// Activate main menu UI.
    /// </summary>
    private void SetMainMenu() => mainMenuParent.SetActive(true);

    /// <summary>
    /// Deactivate main menu UI.
    /// </summary>
    public void UnsetMainMenu() => mainMenuParent.SetActive(false);

    /// <summary>
    /// Activate in-game HUD.
    /// </summary>
    public void SetGameUI() => gameUI.SetActive(true);

    /// <summary>
    /// Deactivate in-game HUD.
    /// </summary>
    public void UnsetGameUI() => gameUI.SetActive(false);
    #endregion

    #region UI Initialization
    /// <summary>
    /// Sets initial values for all HUD elements.
    /// </summary>
    private void UpdateAllUI()
    {
        scoreText.text = "Score: " + ScoreManager.Instance.GetCurrentScore();
        waveText.text = GameManager.Instance.GetCurrentWave().ToString();
        killChainMultiplier.text = KillChainManager.Instance.GetKillChainMultiplier().ToString();
    }
    #endregion

    #region Countdown UI
    /// <summary>
    /// Shows a floating countdown text with feedback.
    /// </summary>
    public void ShowCountdownText(string message)
    {
        var instance = Instantiate(countdownFloatingTextPrefab, countdownSpawnPoint);
        var tmpro = instance.GetComponentInChildren<TMP_Text>();
        if (tmpro != null) tmpro.text = message;

        var feedback = instance.GetComponent<MMF_Player>();
        feedback?.PlayFeedbacks();
    }

    /// <summary>
    /// Clears all countdown text instances.
    /// </summary>
    public void HideCountdownText()
    {
        foreach (Transform child in countdownSpawnPoint)
            Destroy(child.gameObject);
    }
    #endregion
    #region Break Round UI
    /// <summary>
    /// Displays a special message for break rounds (e.g. "System Failure. No Enemies Spawned").
    /// </summary>
    public void ShowBreakRound(int waveNumber)
    {
        ShowFloatingText($"Round {waveNumber}: SYSTEM GLITCH — NO ENEMIES SPAWNED");

        // Optional: Add a separate feedback or animation here
        // You could also trigger a glitch visual/audio using MoreMountains.Feedbacks
    }
    #endregion
    #region Wave Completion UI
    /// <summary>
    /// Displays wave complete text, then hides it after delay.
    /// </summary>
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

    /// <summary>
    /// Shows the upgrade selection UI.
    /// </summary>
    public void ShowUpgradeUI() => upgradeParent.SetActive(true);

    /// <summary>
    /// Hides the upgrade UI.
    /// </summary>
    public void HideUpgradeUI() => upgradeParent.SetActive(false);
    #endregion

    #region Floating Text UI
    /// <summary>
    /// Shows a generic floating text at the specified spawn point.
    /// </summary>
    public void ShowFloatingText(string message)
    {
        var instance = Instantiate(uiFloatingTextPrefab, floatingTextSpawnPoint.parent);
        var rect = instance.GetComponent<RectTransform>();
        rect.anchorMin = floatingTextSpawnPoint.anchorMin;
        rect.anchorMax = floatingTextSpawnPoint.anchorMax;
        rect.pivot = floatingTextSpawnPoint.pivot;
        rect.anchoredPosition = floatingTextSpawnPoint.anchoredPosition;

        var tmpro = instance.GetComponentInChildren<TMP_Text>();
        if (tmpro != null) tmpro.text = message;

        var feedback = instance.GetComponent<MMF_Player>();
        feedback?.PlayFeedbacks();

        Destroy(instance, 2f);
    }
    #endregion

    #region Upgrade Controls
    /// <summary>
    /// Enables or disables upgrade buttons based on availability.
    /// </summary>
    private void ShowUpgradeControls(bool isAvailable)
    {
        //readyButton.gameObject.SetActive(isAvailable);
        upgradeHoldButtonParent.SetActive(isAvailable);
        if (!isAvailable) SetUpgradeHoldProgress(0f);
    }

    /// <summary>
    /// Updates fill amount of upgrade hold slider.
    /// </summary>
    public void SetUpgradeHoldProgress(float normalized)
    {
        upgradeHoldImage.fillAmount = Mathf.Clamp01(normalized);
    }

    /// <summary>
    /// Pauses game and shows upgrade menu.
    /// </summary>
    public void ShowUpgradeMenu()
    {
        TimeManager.Instance.PauseGame();
        upgradeParent.SetActive(true);
    }

    /// <summary>
    /// Hides upgrade menu and resumes game.
    /// </summary>
    public void HideUpgradeMenu()
    {
        upgradeParent.SetActive(false);
        GameManager.Instance.PlayerReadyForNextWave();
        //TimeManager.Instance.ResumeGame();
    }
    #endregion

    #region Button Setup
    /// <summary>
    /// Configures button click listeners.
    /// </summary>
    private void SetupButtons()
    {
        startButton.onClick.AddListener(GameManager.Instance.StartGame);
        readyButton.onClick.AddListener(HideUpgradeMenu);
    }
    #endregion

    #region References
    /// <summary>
    /// Returns the main UI canvas.
    /// </summary>
    public Canvas GetUICanvas() => uiCanvas;
    #endregion
}
