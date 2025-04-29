using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private LayerMask ignoreLayerMask;
    private Vector3 newPos;
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        int ignoreLayer = 6;
        ignoreLayerMask = ~(1 << ignoreLayer);  // ~ inverts bits, so it ignores the specified layer
    }

    private void Update()
    {
        newPos = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        lineRenderer.SetPosition(0, newPos);

        RaycastHit hit;
        if (Physics.Raycast(newPos, transform.forward, out hit, Mathf.Infinity, ignoreLayerMask))
        {
            if (hit.collider)
            {
                lineRenderer.SetPosition(1, hit.point);
            }
        }
        else
        {
            lineRenderer.SetPosition(1, newPos + transform.forward * 5000);
        }
    }
}
