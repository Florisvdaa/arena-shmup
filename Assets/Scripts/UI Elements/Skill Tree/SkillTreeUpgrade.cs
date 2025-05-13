using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeUpgrade : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data")]
    [Tooltip("Defines name, cost, icon, and effects")]
    [SerializeField] private UpgradeNodeDefinition nodeDef;

    [Header("UI References")]
    [Tooltip("The icon to tint/replace")]
    [SerializeField] private Image iconImage;

    [Header("Tooltip")]
    [Tooltip("Prefab of the UpgradeTooltip panel")]
    [SerializeField] private GameObject tooltipPrefab;
    [Tooltip("Parent canvas for tooltips")]
    [SerializeField] private Canvas uiCanvas;
    [Tooltip("Offset (in canvas local units) from the cursor")]
    [SerializeField] private Vector2 tooltipOffset = new Vector2(10, -10);

    [Header("Fill Settings")]
    [Tooltip("Image (child) set to Filled in Inspector")]
    [SerializeField] private Image fillImage;
    [SerializeField] private float fillDuration = 1f;
    [SerializeField] private UnityEvent onFillComplete;

    // Internals
    private float fillTimer = 0f;
    private bool isFilling = false;
    private bool isFilled = false;
    private UITooltip uiTooltipInstance;
    private GameObject currentToolTip;
    void Awake()
    {
        // Ensure fill‐type
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Vertical;
        fillImage.fillAmount = 0f;
    }

    void Start()
    {
        // Populate UI from the SO
        if (nodeDef != null)
            ApplyDefinition();
        if (uiCanvas == null)
            uiCanvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (isFilling && !isFilled)
        {
            fillTimer += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(fillTimer / fillDuration);

            if (fillTimer >= fillDuration)
            {
                isFilled = true;
                isFilling = false;
                fillImage.fillAmount = 1f;

                // 1) Invoke the SO’s own effect event
                nodeDef.onPurchased?.Invoke();
                // 2) Hook for any extra UI behavior
                onFillComplete?.Invoke();
            }
        }

        if (uiTooltipInstance != null)
        {
            // Convert screen mouse pos to canvas local pos
            RectTransform canvasRT = uiCanvas.transform as RectTransform;
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRT,
                Input.mousePosition,
                uiCanvas.renderMode == RenderMode.ScreenSpaceCamera ? uiCanvas.worldCamera : null,
                out localPoint
            );
            uiTooltipInstance.GetComponent<RectTransform>().anchoredPosition = localPoint + tooltipOffset;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isFilled) return;
        isFilling = true;
        fillTimer = 0f;
        fillImage.fillAmount = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isFilled) return;
        isFilling = false;
        fillTimer = 0f;
        fillImage.fillAmount = 0f;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("do i get here");

        if (tooltipPrefab == null || nodeDef == null) return;

        // Instantiate tooltip under the canvas
        currentToolTip = Instantiate(tooltipPrefab, uiCanvas.transform);
        uiTooltipInstance = currentToolTip.GetComponent<UITooltip>();
        uiTooltipInstance.Show(nodeDef.nodeName,nodeDef.description, nodeDef.cost);

        UpdateTooltipPosition();
    }
    private void UpdateTooltipPosition()
    {
        RectTransform canvasRT = uiCanvas.transform as RectTransform;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT,
            Input.mousePosition,
            uiCanvas.renderMode == RenderMode.ScreenSpaceCamera ? uiCanvas.worldCamera : null,
            out localPoint
        );
        uiTooltipInstance.GetComponent<RectTransform>().anchoredPosition = localPoint + tooltipOffset;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"[SkillTreeUpgrade] OnPointerExit on {name}");
        if (uiTooltipInstance != null)
        {
            Destroy(uiTooltipInstance.gameObject);
            Destroy(currentToolTip);
        }
            
    }
    private void ApplyDefinition()
    {
        iconImage.sprite = nodeDef.icon;
        iconImage.color = Color.white;  // reset tint in case you highlight on hover
    }
}
