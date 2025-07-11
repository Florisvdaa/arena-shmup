using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBomb : MonoBehaviour
{
    [SerializeField] private float delayBeforeExplosion = 1f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private int explosionDamage = 5;
    [SerializeField] private GameObject explosionVFX;

    private void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeExplosion);

        if (explosionVFX) Instantiate(explosionVFX, transform.position, Quaternion.identity);

        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hitEnemies)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(explosionDamage);
            }
        }

        Destroy(gameObject);
    }
}
