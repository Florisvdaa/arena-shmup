using FXV;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private float bulletHitSize = 1f;
    [SerializeField] private float bulletHitDuration = 2f;
    private int Damage => Mathf.RoundToInt(PlayerSettings.Instance.CurrentFireDamage);

    private float timer;

    void OnEnable()
    {
        timer = lifetime;

        // Reset velocity for safety if using rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ReturnToPool();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"[Bullet] Trigger entered with: {other.name}");

        if (other.CompareTag("Enemy"))
        {
            //Debug.Log("[Bullet] Hit Enemy");
            other.GetComponent<Enemy>()?.TakeDamage(Damage);
            SpawnImpact();
            ReturnToPool();
        }
        else if (other.CompareTag("Wall"))
        {
            //Debug.Log("[Bullet] Hit Wall");
            Shield shield = other.GetComponentInParent<Shield>();
            if (shield != null)
            {
                //Debug.Log("[Bullet] Hit Shield");
                shield.OnHit(transform.position, -transform.forward, bulletHitSize, bulletHitDuration);
            }

            SpawnImpact();
            ReturnToPool();
        }
    }

    private void SpawnImpact()
    {
        if (impactPrefab)
        {
            GameObject fx = Instantiate(impactPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 1f);
        }
    }

    private void ReturnToPool()
    {
        BulletPool.Instance.ReturnBullet(this.gameObject);
    }
}
