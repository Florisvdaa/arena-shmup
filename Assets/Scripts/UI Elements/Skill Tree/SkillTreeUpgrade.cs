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
    private bool isUnlocked = false;

    private Color iconDefaultColor;
    private Color fillDefaultColor;

    private UITooltip uiTooltipInstance;
    private GameObject currentTooltip;

    void Awake()
    {
        // Ensure fill‐type
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Vertical;
        fillImage.fillAmount = 0f;

        // Capture whatever color you've set in the Inspector
        iconDefaultColor = iconImage.color;
        fillDefaultColor = fillImage.color;
    }

    void Start()
    {
        // If you forgot your Canvas reference
        if (uiCanvas == null)
            uiCanvas = GetComponentInParent<Canvas>();

        // Populate icon sprite & reset tint
        if (nodeDef != null)
            ApplyDefinition();

        // Initial lock/unlock pass
        EvaluatePrerequisites();

        // Listen for other upgrades unlocking prerequisites
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OnUpgradePurchased += OnSomeNodePurchased;
    }

    void OnDestroy()
    {
        if (UpgradeManager.Instance != null)
            UpgradeManager.Instance.OnUpgradePurchased -= OnSomeNodePurchased;
    }

    void Update()
    {
        // 1) filling logic
        if (isUnlocked && isFilling && !isFilled)
        {
            fillTimer += Time.deltaTime;
            fillImage.fillAmount = Mathf.Clamp01(fillTimer / fillDuration);

            if (fillTimer >= fillDuration)
            {
                isFilled = true;
                isFilling = false;
                fillImage.fillAmount = 1f;

                ApplyEffects(); // ← Apply effects based on SO
                //nodeDef.onPurchased?.Invoke(); // Optional extras
                UpgradeManager.Instance?.RegisterPurchase(nodeDef);
                onFillComplete?.Invoke();
            }
        }

        // 2) hover‐tooltip follow
        if (uiTooltipInstance != null)
            UpdateTooltipPosition();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isUnlocked || isFilled) return;

        isFilling = true;
        fillTimer = 0f;
        fillImage.fillAmount = 0f;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isUnlocked || isFilled) return;

        isFilling = false;
        fillTimer = 0f;
        fillImage.fillAmount = 0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isUnlocked || tooltipPrefab == null) return;

        currentTooltip = Instantiate(tooltipPrefab, uiCanvas.transform, false);
        uiTooltipInstance = currentTooltip.GetComponent<UITooltip>();
        uiTooltipInstance.Show(nodeDef.nodeName, nodeDef.description, nodeDef.cost);

        UpdateTooltipPosition();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (uiTooltipInstance != null)
            Destroy(currentTooltip);
    }

    private void OnSomeNodePurchased(UpgradeNodeDefinition purchasedDef)
    {
        // if one of my prerequisites was just purchased, re-check
        foreach (var prereq in nodeDef.prerequisites)
        {
            if (prereq == purchasedDef)
            {
                EvaluatePrerequisites();
                break;
            }
        }
    }

    private void EvaluatePrerequisites()
    {
        // unlocked if no prereqs or *all* prereqs purchased
        isUnlocked = true;
        foreach (var prereq in nodeDef.prerequisites)
        {
            if (UpgradeManager.Instance == null ||
                !UpgradeManager.Instance.IsPurchased(prereq))
            {
                isUnlocked = false;
                break;
            }
        }

        // apply color: default vs gray
        var gray = new Color(0.5f, 0.5f, 0.5f);
        iconImage.color = isUnlocked ? iconDefaultColor : gray;
        fillImage.color = isUnlocked ? fillDefaultColor : gray;
    }

    private void UpdateTooltipPosition()
    {
        var canvasRT = (RectTransform)uiCanvas.transform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRT,
            Input.mousePosition,
            uiCanvas.renderMode == RenderMode.ScreenSpaceCamera
                ? uiCanvas.worldCamera
                : null,
            out Vector2 localPoint
        );
        uiTooltipInstance.GetComponent<RectTransform>().anchoredPosition
            = localPoint + tooltipOffset;
    }

    private void ApplyDefinition()
    {
        iconImage.sprite = nodeDef.icon;
        // restore the default tint you captured
        iconImage.color = iconDefaultColor;
    }

    private void ApplyEffects()
    {
        foreach (var effect in nodeDef.effects)
        {
            switch (effect.effectType)
            {
                case UpgradeEffectType.HealthPercentage:
                    PlayerSettings.Instance.IncreaseHealthByPercentage(effect.value);
                    break;
                case UpgradeEffectType.SpeedFlat:
                    PlayerSettings.Instance.IncreaseSpeed(effect.value);
                    break;
                case UpgradeEffectType.FireRateDecrease:
                    PlayerSettings.Instance.IncreaseFireRate(effect.value);
                    break;
                case UpgradeEffectType.ExpMultiplier:
                    PlayerSettings.Instance.IncreaseExpMultiplier(effect.value);
                    break;
            }
        }
    }
}
