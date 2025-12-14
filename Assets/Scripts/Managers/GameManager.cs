using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages game state including start sequence, wave progression, and UI transitions.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else Instance = this;
    }
}
