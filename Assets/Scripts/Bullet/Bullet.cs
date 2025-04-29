using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private int damage = 1;
    private float timer;
    void OnEnable()
    {
        timer = lifetime;
    }
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ReturnToPool();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // TODO: damage or effects
        //gameObject.SetActive(false);
        if (other.gameObject.CompareTag("Wall"))
        {
            Debug.Log("hit the wall");

            SpawnImpact();
            ReturnToPool();
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>()?.TakeDamage(damage);
            SpawnImpact();
            ReturnToPool();
        }
    }
    private void SpawnImpact()
    {
        GameObject fx = Instantiate(impactPrefab, transform.position, Quaternion.identity);
        fx.SetActive(true);

        // Optional: destroy after it finishes
        Destroy(fx, 1f);
    }
    private void ReturnToPool()
    {
        BulletPool.Instance.ReturnBullet(this.gameObject);
    }
}
