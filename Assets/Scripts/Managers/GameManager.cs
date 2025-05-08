using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages game state including start sequence, wave progression, and UI transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Global access point for the GameManager.
    /// </summary>
    public static GameManager Instance { get; private set; }
    #endregion

    #region Inspector Fields
    [Header("Player Reference")]
    [Tooltip("Player GameObject in the scene.")]
    [SerializeField] private GameObject player;
    [Tooltip("Transform where the player is tracked during gameplay.")]
    [SerializeField] private Transform currentPlayerTransform;
    [Tooltip("Starting position for the player at game start.")]
    [SerializeField] private Transform playerStartPosition;

    [Header("Wave Info")]
    [Tooltip("Base number of enemies in the first wave.")]
    [SerializeField] private int baseEnemiesPerWave = 5;
    #endregion

    #region Private Fields
    private int currentWave = 0;
    private bool canPlayerMove = false;
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize singleton instance or destroy duplicates.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Update player tracking transform every frame.
    /// </summary>
    private void Update()
    {
        currentPlayerTransform.position = player.transform.position;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Begins game start sequence: hides menu, sets camera, and starts countdown.
    /// </summary>
    public void StartGame()
    {
        UIManager.Instance.UnsetMainMenu();
        CameraSwitcher.Instance.SetGameCam();
        StartCoroutine(GameStartCountdown());
    }

    /// <summary>
    /// Initiates the next enemy wave with scaled enemy count.
    /// </summary>
    public void StartNextWave()
    {
        currentWave++;
        int enemiesThisWave = baseEnemiesPerWave + (currentWave - 1) * 2;
        SpawnManager.Instance.StartWave(100); // DEBUG
        //SpawnManager.Instance.StartWave(enemiesThisWave);
    }

    /// <summary>
    /// Called when all enemies in the current wave die; triggers completion sequence.
    /// </summary>
    public void OnWaveComplete()
    {
        StartCoroutine(OnWaveCompleteCoroutine());
    }

    /// <summary>
    /// Called after upgrade UI; restarts countdown and wave.
    /// </summary>
    public void PlayerReadyForNextWave()
    {
        UIManager.Instance.HideUpgradeUI();
        CameraSwitcher.Instance.SetGameCam();
        StartCoroutine(GameStartCountdown());
    }
    #endregion

    #region Countdown Sequence
    /// <summary>
    /// Handles player repositioning and countdown before wave starts.
    /// </summary>
    private IEnumerator GameStartCountdown()
    {
        yield return new WaitForSeconds(0.5f);

        // Lerp player to start position
        float lerpDuration = 2f;
        float elapsedTime = 0f;
        Vector3 startPos = player.transform.position;
        Vector3 targetPos = playerStartPosition.position;

        while (elapsedTime < lerpDuration)
        {
            player.transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / lerpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.transform.position = targetPos;

        // Countdown display
        string[] sequence = { "3", "2", "1", "GOOO!" };
        foreach (string s in sequence)
        {
            UIManager.Instance.ShowCountdownText(s);
            yield return new WaitForSeconds(1f);
        }

        canPlayerMove = true;
        UIManager.Instance.HideCountdownText();
        UIManager.Instance.SetGameUI();
        StartNextWave();
    }
    #endregion

    #region Wave Completion Sequence
    /// <summary>
    /// Shows wave complete UI and transitions to upgrade state.
    /// </summary>
    private IEnumerator OnWaveCompleteCoroutine()
    {
        canPlayerMove = false;
        UIManager.Instance.ShowWaveComplete();
        yield return new WaitForSeconds(2f);
        UIManager.Instance.ShowUpgradeUI();
    }
    #endregion

    #region Accessors
    /// <summary>
    /// Returns whether the player can currently move.
    /// </summary>
    public bool GetCanPlayerMove() => canPlayerMove;

    /// <summary>
    /// Returns the current wave index.
    /// </summary>
    public int GetCurrentWave() => currentWave;

    /// <summary>
    /// Returns the player's transform.
    /// </summary>
    public Transform GetPlayerTransform() => currentPlayerTransform;
    #endregion
}
