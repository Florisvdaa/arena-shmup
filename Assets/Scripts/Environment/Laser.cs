using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour, IHazard
{
    [Header("Laser Settings")]
    [SerializeField] private float range = 2;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float damageTickRate = 1f;
    [SerializeField] private float activeDuration = 5f;
    [SerializeField] private float distanceFromGround = 0.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private bool isRotatingLaser = false;
    [SerializeField] private GameObject laserVisual;

    private LineRenderer lineRenderer;

    private Dictionary<IDamageable, float> lastDamageTime = new Dictionary<IDamageable, float>();

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
        StartCoroutine(LaserActiveRoutine());
    }

    public void Deactivate()
    {
        lineRenderer.enabled = false;
    }

    private void Update()
    {
        if (!lineRenderer.enabled) return;

        if (isRotatingLaser)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

            // Now rotates the visual but can later be model opening and spawning laser animation
            laserVisual.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
        
        UpdateLaserPosition();
    }

    private IEnumerator LaserActiveRoutine()
    {
        float timer = activeDuration;

        while (timer > 0f) 
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        Deactivate();
    }

    private void UpdateLaserPosition()
    {
        Vector3 center = transform.position + Vector3.up * distanceFromGround;
        Vector3 leftPos = center - transform.right * range;
        Vector3 rightPos = center + transform.right * range;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, leftPos);
        lineRenderer.SetPosition(1, rightPos);

        CheckLaserHits(leftPos, rightPos);
    }

    private void CheckLaserHits(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);

        RaycastHit[] hits = Physics.RaycastAll(start, direction, distance);

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                float currentTime = Time.time;

                if (!lastDamageTime.ContainsKey(damageable))
                    lastDamageTime[damageable] = -999f;

                if (currentTime - lastDamageTime[damageable] >= damageTickRate)
                {
                    damageable.Damage(damageAmount);
                    lastDamageTime[damageable] = currentTime;
                }

            }
        }
    }

    public bool IsActive => lineRenderer.enabled;
}
