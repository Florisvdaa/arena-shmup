using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLaser : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float laserDistance = 8f;
    [SerializeField] private LayerMask ignoreMask;

    private RaycastHit raycastHit;
    private Ray ray;

    private void Start()
    {
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        ray = new(transform.position, transform.forward);

        if (Physics.Raycast(ray, out raycastHit, laserDistance, ~ignoreMask))
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, raycastHit.point); // Sets the end of the linerenderer to the hit pos.
        }
        else
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.forward * laserDistance);
        }
    }
    
}
