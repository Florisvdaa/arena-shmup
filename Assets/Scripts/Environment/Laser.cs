using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float range = 2;
    [SerializeField] private float distanceFromGround = 0.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float activeTimer = 10f; // Active for duration
    [SerializeField] private bool isRotatingLaser = false;
    [SerializeField] private bool isLaserActive = false;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void ActivateLaser()
    {
        lineRenderer.enabled = true;
        isLaserActive = true;
        //UpdateLaserPosition();
    }

    private void Update()
    {
        if (!lineRenderer.enabled) return;
        
        if (isRotatingLaser)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        if (isLaserActive )
            UpdateLaserPosition();
    }

    public void DeactivateLaser()
    {
        lineRenderer.enabled = false;
        isLaserActive = false;
    }

    private void UpdateLaserPosition()
    {
        Vector3 center = new Vector3(transform.position.x, transform.position.y + distanceFromGround, transform.position.z);
        Vector3 leftPos = center - transform.right * range;
        Vector3 rightPos = center + transform.right * range;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, leftPos);
        lineRenderer.SetPosition(1, rightPos);
    }

}
