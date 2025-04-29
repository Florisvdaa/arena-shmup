using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public static FloatingText Instance { get; private set; }
    [SerializeField] private GameObject uiFloatingTextPrefab;
    private Canvas uiCanvas;
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
        uiCanvas = UIManager.Instance.GetUICanvas();
    }
    public void ShowFloatingText(string message, Vector3 worldPosition)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        GameObject instance = Instantiate(uiFloatingTextPrefab, uiCanvas.transform);

        RectTransform rectTransform = instance.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = screenPos;

        var textComponent = instance.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = message;
        }

        var mmfPlayer = instance.GetComponent<MMF_Player>();
        if (mmfPlayer != null)
        {
            mmfPlayer.PlayFeedbacks();
        }

        Destroy(instance, 2f); // Auto-cleanup after feedbacks
    }
}
