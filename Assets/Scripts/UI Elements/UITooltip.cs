using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITooltip : MonoBehaviour
{
    [Tooltip("Drag the child Text that shows the node name")]
    [SerializeField] private TextMeshProUGUI nameText;
    [Tooltip("Drag the child Text that shows the cost")]
    [SerializeField] private TextMeshProUGUI costText;
    [Tooltip("Drag the child Text that shows the description")]
    [SerializeField] private TextMeshProUGUI descriptiontext;

    /// <summary>
    /// Fill in the name and cost, then enable the tooltip.
    /// </summary>
    public void Show(string nodeName, string description, int cost)
    {
        nameText.text = nodeName;
        descriptiontext.text = description;
        costText.text = $"Cost: {cost}";
        gameObject.SetActive(true);
    }
}
