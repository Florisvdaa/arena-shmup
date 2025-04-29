using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Player Reference")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform currentPlayerTransform;
    [SerializeField] private Transform playerStartPosition;

    [Header("Wave Info")]
    private int currentWave = 0;
    [SerializeField] private int baseEnemiesPerWave = 5;

    private bool canPlayerMove = false;

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
    }
    public void StartGame()
    {
        UIManager.Instance.UnsetMainMenu();
        CameraSwitcher.Instance.SetGameCam();
        StartCoroutine(GameStartCountdown());
    }
    #region Countdown
    private IEnumerator GameStartCountdown()
    {
        yield return new WaitForSeconds(0.5f); // Small delay before repositioning

        // Start lerping the player to the start position
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

        // Snap to final position just in case
        player.transform.position = targetPos;

        // Countdown
        string[] countdownSequence = { "3", "2", "1", "GOOO!" };
        foreach (var c in countdownSequence)
        {
            UIManager.Instance.ShowCountdownText(c);
            yield return new WaitForSeconds(1f);
        }

        canPlayerMove = true;
        UIManager.Instance.HideCountdownText();
        UIManager.Instance.SetGameUI();
        StartNextWave();
    }
    public void StartNextWave()
    {
        currentWave++;
        int enemiesThisWave = baseEnemiesPerWave + (currentWave - 1) * 2;
        SpawnManager.Instance.StartWave(enemiesThisWave);
    }
    public void OnWaveComplete()
    {
        StartCoroutine(OnWaveCompleteCoroutine());
    }
    private IEnumerator OnWaveCompleteCoroutine() 
    {
        canPlayerMove = false;

        UIManager.Instance.ShowWaveComplete();
        yield return new WaitForSeconds(2f);
        CameraSwitcher.Instance.SetUpgradeCam();
        yield return new WaitForSeconds(1f);
        UIManager.Instance.ShowUpgradeUI();
    }

    public void PlayerReadyForNextWave()
    {
        UIManager.Instance.HideUpgradeUI();
        CameraSwitcher.Instance.SetGameCam();
        StartCoroutine(GameStartCountdown());
    }
    #endregion
    private void Update()
    {
        currentPlayerTransform.position = player.transform.position;
    }
    #region References
    public Transform GetPlayerTransform() => currentPlayerTransform;
    public bool GetCanPlayerMove() => canPlayerMove;
    #endregion
}
