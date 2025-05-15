using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages game state including start sequence, wave progression, and UI transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Inspector Fields
    [Header("Player Reference")]
    [Tooltip("Player GameObject in the scene.")]
    [SerializeField] private GameObject player;
    [Tooltip("Transform where the player is tracked during gameplay.")]
    [SerializeField] private Transform currentPlayerTransform;
    [Tooltip("Starting position for the player at game start or new wave.")]
    [SerializeField] private Transform playerStartPosition;
    #endregion

    #region Private Fields
    private int currentWave = 0;
    private bool canPlayerMove;
    #endregion

    #region Public API
    /// <summary>
    /// How many enemies the player actually killed in the last wave.
    /// </summary>
    public int LastWaveKills { get; internal set; }

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
        UIManager.Instance.HideUpgradeUI();
        CameraSwitcher.Instance.SetGameCam();
        ProgressManager.Instance.ClearRemainingPUP();
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
        yield return new WaitForSeconds(0.5f);
        ProgressManager.Instance.EndRound();
        UIManager.Instance.ShowUpgradeUI();
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
