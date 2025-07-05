using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages game state including start sequence, wave progression, and UI transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject player;
    [SerializeField] private Transform currentPlayerTransform;
    [SerializeField] private Transform playerStartPosition;

    private int currentWave = 0;
    private bool canPlayerMove;

    #region Public API
    /// <summary>
    /// How many enemies the player actually killed in the last wave.
    /// </summary>
    public int LastWaveKills { get; internal set; }

    private void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
        UIManager.Instance.UnsetMainMenu();
        CameraSwitcher.Instance.SetGameCam();
        StartCoroutine(GameStartCountdown());
    }

    public void StartNextWave()
    {
        currentWave++;
        SpawnManager.Instance.StartWave(currentWave);
        canPlayerMove = true;
    }

    public void OnWaveComplete()
    {
        canPlayerMove = false;
        StartCoroutine(OnWaveCompleteCoroutine());
    }
    public void OnBreakRound(int waveNumber)
    {
        canPlayerMove = false;
        UIManager.Instance.ShowBreakRound(waveNumber);
    }
    public void PlayerReadyForNextWave()
    {
        //UIManager.Instance.HideUpgradeUI();
        CameraSwitcher.Instance.SetGameCam();
        StartCoroutine(GameStartCountdown());
    }
    public void PlayerChoseUpgrade()
    {
        StartCoroutine(GameStartCountdown());
    }
    #endregion

    #region Countdown / Completion
    private IEnumerator GameStartCountdown()
    {
        //ResetPlayerPosition();
        FeedBackManager.Instance.PlayTeleportToStartPosFeedback();

        canPlayerMove = false;
        yield return new WaitForSeconds(1f);
        FeedBackManager.Instance.PlayCountDownFeedback();
        yield return new WaitWhile(() => FeedBackManager.Instance.CountDownPlayer().IsPlaying);
        yield return new WaitForEndOfFrame();
        FeedBackManager.Instance.WaveIndicatorFeedback();

        UIManager.Instance.HideCountdownText();
        UIManager.Instance.SetGameUI();
        StartNextWave();
    }

    private IEnumerator OnWaveCompleteCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        UpgradeManager.Instance.ShowUpgradeOptions();

        //StartCoroutine(GameStartCountdown());

        //ProgressManager.Instance.EndRound();
        //UIManager.Instance.ShowUpgradeUI();
    }
    #endregion
    #region Unity Callbacks
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        if (player != null && currentPlayerTransform != null)
            currentPlayerTransform.position = player.transform.position;
    }
    #endregion

    #region Accessors
    public bool GetCanPlayerMove() => canPlayerMove;
    public int GetCurrentWave() => currentWave;
    public Transform GetPlayerTransform() => currentPlayerTransform;
    #endregion
}
