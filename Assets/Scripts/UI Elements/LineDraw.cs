using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineDraw : MonoBehaviour
{
    [Header("UI Canvas (for sorting)")]
    [Tooltip("Your Screen-Space–Camera or World-Space Canvas")]
    public Canvas uiCanvas;

    [Header("Endpoint (UI Element)")]
    [Tooltip("Drag in the RectTransform you want to draw to")]
    public RectTransform endPoint;

    [Header("Line Settings")]
    [Tooltip("Material to use for the line")]
    public Material lineMaterial;

    [Tooltip("Thickness in world units")]
    public float thickness = 0.05f;

    [Tooltip("Duration of the draw animation (seconds)")]
    public float drawDuration = 1f;

    [Tooltip("Automatically animate on Start()")]
    public bool playOnStart = false;

    private LineRenderer lr;
    private bool isAnimating = false;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();

        // Assign material (or fallback to UI/Default)
        if (lineMaterial != null)
            lr.material = lineMaterial;
        else
            lr.material = new Material(Shader.Find("UI/Default"));

        lr.useWorldSpace = true;
        lr.startWidth = lr.endWidth = thickness;

        // Match the Canvas’s sorting so the line draws on top
        if (uiCanvas != null)
        {
            lr.sortingLayerName = uiCanvas.sortingLayerName;
            lr.sortingOrder = uiCanvas.sortingOrder + 1;
        }
    }

    void Start()
    {
        if (playOnStart)
            PlayDraw();
        else
            DrawFullLine();
    }

    void LateUpdate()
    {
        // If not animating, keep the line “static” from start → end
        if (!isAnimating && endPoint != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, endPoint.position);
        }
    }

    /// <summary>
    /// Call this to animate the line drawing from this.transform.position → endPoint.position
    /// </summary>
    public void PlayDraw()
    {
        if (endPoint == null) return;
        StopAllCoroutines();
        StartCoroutine(DrawLineCoroutine());
    }

    /// <summary>
    /// Immediately sets the line fully drawn
    /// </summary>
    public void DrawFullLine()
    {
        if (endPoint == null) return;
        StopAllCoroutines();
        isAnimating = false;
        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, endPoint.position);
    }

    private IEnumerator DrawLineCoroutine()
    {
        isAnimating = true;
        lr.positionCount = 2;

        Vector3 start = transform.position;
        Vector3 end = endPoint.position;
        float elapsed = 0f;

        while (elapsed < drawDuration)
        {
            float t = elapsed / drawDuration;
            Vector3 current = Vector3.Lerp(start, end, t);

            lr.SetPosition(0, start);
            lr.SetPosition(1, current);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // finish at the exact endpoint
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        isAnimating = false;
    }
}
