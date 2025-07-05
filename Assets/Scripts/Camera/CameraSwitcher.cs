using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// Switches between main menu and gameplay virtual cameras.
/// </summary>
public class CameraSwitcher : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Global access point to the CameraSwitcher instance.
    /// </summary>
    public static CameraSwitcher Instance { get; private set; }
    #endregion

    #region Inspector Fields
    [Tooltip("Virtual camera used for the main menu screen.")]
    [SerializeField] private CinemachineVirtualCamera mainMenuCam;

    [Tooltip("Virtual camera used during gameplay.")]
    [SerializeField] private CinemachineVirtualCamera gameCam;
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize singleton and destroy duplicates.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Set the default camera at start.
    /// </summary>
    private void Start()
    {
        //SetMainMenuCam();
        SetGameCam();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Activate the main menu camera and deactivate the game camera.
    /// </summary>
    public void SetMainMenuCam()
    {
        mainMenuCam.Priority = 10;
        gameCam.Priority = 0;
    }

    /// <summary>
    /// Activate the game camera and deactivate the main menu camera.
    /// </summary>
    public void SetGameCam()
    {
        gameCam.Priority = 10;
        mainMenuCam.Priority = 0;
    }
    #endregion
}
