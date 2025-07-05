using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Settings")]
    [SerializeField] private int highScore = 0;
    [SerializeField] private int score = 0;
    [SerializeField] private int scoreMultipier = 0;
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
    public void AddScore(int amount)
    {
        score += amount;
        UIManager.Instance.ShowFloatingText(amount.ToString());
    }

    #region References
    public int GetCurrentScore() => score;
    #endregion
}
