using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages global time control: slow motion, pause, and normal time flow.
/// </summary>
public class TimeManager : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Global access to the TimeManager instance.
    /// </summary>
    public static TimeManager Instance { get; private set; }
    #endregion

    #region Inspector Fields
    [Tooltip("Scale factor applied when entering slow motion (0 = frozen, 1 = normal speed).")]
    [Range(0f, 1f)]
    public float slowMoScale = 0.2f;
    #endregion

    #region Private Fields
    private float _originalFixedDelta;
    /// <summary>
    /// Indicates whether the game is currently paused.
    /// </summary>
    public bool IsPaused { get; private set; }
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize singleton, prevent duplicates, and cache original FixedDeltaTime.
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
        _originalFixedDelta = Time.fixedDeltaTime;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Enter slow motion mode by scaling down time and fixed delta.
    /// </summary>
    public void EnterSlowMotion()
    {
        IsPaused = false;
        Time.timeScale = slowMoScale;
        Time.fixedDeltaTime = _originalFixedDelta * slowMoScale;
    }

    /// <summary>
    /// Exit slow motion, restoring real-time scales.
    /// </summary>
    public void ExitSlowMotion()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = _originalFixedDelta;
        IsPaused = false;
    }

    /// <summary>
    /// Pause the game by stopping time.
    /// </summary>
    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        // Once paused, fixedDeltaTime is irrelevant until resume
    }

    /// <summary>
    /// Resume the game from pause, restoring time scales.
    /// </summary>
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = _originalFixedDelta;
        IsPaused = false;
    }
    #endregion
}
