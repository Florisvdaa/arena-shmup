using UnityEngine;

public class Laser : MonoBehaviour, IHazard
{
    [SerializeField] private float range = 2;
    [SerializeField] private float distanceFromGround = 0.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool isRotatingLaser = false;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        lineRenderer.enabled = true;
    }

    public void Deactivate()
    {
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (!lineRenderer.enabled) return;

        if (isRotatingLaser)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        UpdateLaserPosition();
    }

    private void UpdateLaserPosition()
    {
        Vector3 center = transform.position + Vector3.up * distanceFromGround;
        Vector3 leftPos = center - transform.right * range;
        Vector3 rightPos = center + transform.right * range;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, leftPos);
        lineRenderer.SetPosition(1, rightPos);
    }
}
