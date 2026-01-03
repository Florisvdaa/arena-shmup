using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ShockPlate : MonoBehaviour, IHazard
{
    [Header("ShockPlate Settings")]
    [SerializeField] private float chargeTime = 2f;
    [SerializeField] private float shockDuration = 1f;
    [SerializeField] private Transform plateCenter;
    [SerializeField] private float damageRadius;
    [SerializeField] private int damage;
    [SerializeField] private LayerMask damageMask;

    [Header("Visual Settings")]
    [SerializeField] private List<Renderer> plateRenderers;
    [SerializeField] private Color chargedColor = Color.cyan;
    [SerializeField] private float colorLerpSpeed = 5f;

    private bool isShocking = false;
    private HashSet<IDamageable> shockedTargets = new HashSet<IDamageable>();
    private List<Material> originalMaterials = new List<Material>();
    private List<Material> runtimeMaterials = new List<Material>();

    private void Awake()
    {
        // Dupe materials so we dont modify them
        foreach (var rend in plateRenderers)
        {
            Material matInstance = new Material(rend.material);
            runtimeMaterials.Add(matInstance);
            originalMaterials.Add(new Material(rend.material)); // Store orignal
            rend.material = matInstance;
        }
    }

    public void Activate()
    {
        if(!isShocking)
            StartCoroutine(ShockRoutine());
        //Shock();
    }

    private IEnumerator ShockRoutine()
    {
        isShocking = true;
        shockedTargets.Clear();

        // Charge up
        StartCoroutine(LerpPlateColor(chargedColor));
        yield return new WaitForSeconds(chargeTime);

        // Shock Active
        Debug.Log("Shock Active");
        float timer = shockDuration;

        while (timer > 0f)
        {
            ApplyShock();
            timer -= Time.deltaTime;
            yield return null;
        }

        // End, return to normal visuals
        StartCoroutine(LerpPlateColor(originalMaterials[0].color));
        isShocking = false;
    }

    private IEnumerator LerpPlateColor(Color targetColor)
    {
        bool done = false;

        while (!done) 
        {
            done = true;

            for (int i = 0; i < runtimeMaterials.Count; i++) 
            {
                Material mat = runtimeMaterials[i];
                Color newColor = Color.Lerp(mat.color, targetColor, Time.deltaTime * colorLerpSpeed);

                if (((Vector4)newColor - (Vector4)targetColor).sqrMagnitude > 0.001f) { done = false; }

                mat.color = newColor;
            }
            yield return null;
        }
    }

    private void ApplyShock()
    {
        Debug.Log("Shock!");

        Collider[] hits = Physics.OverlapSphere(plateCenter.position, damageRadius, damageMask);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                if (!shockedTargets.Contains(damageable))
                {
                    shockedTargets.Add(damageable);
                    damageable.Damage(damage);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan; 
        Gizmos.DrawWireSphere(plateCenter.position, damageRadius);
    }

    public void Deactivate()
    {
        // Optional: add behavior
    }
}
