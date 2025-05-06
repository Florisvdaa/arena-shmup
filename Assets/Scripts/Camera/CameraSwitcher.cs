using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher Instance { get; private set; }
    [SerializeField] private CinemachineVirtualCamera mainMenuCam;
    [SerializeField] private CinemachineVirtualCamera gameCam;

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
    private void Start()
    {
        // default cam
        SetMainMenuCam();
    }

    public void SetMainMenuCam()
    {
        mainMenuCam.Priority= 10;
        gameCam.Priority= 0;
    }
    public void SetGameCam()
    {
        gameCam.Priority = 10;
        mainMenuCam.Priority = 0;
    }
}
