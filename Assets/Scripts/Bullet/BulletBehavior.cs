using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float normalBulletSpeed = 25f;
    [SerializeField] private float normalDestroyTime = 1f;
    [SerializeField] private GameObject hitParticleSystem;

    private int damage;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.AddForce(transform.forward * normalBulletSpeed, ForceMode.VelocityChange);

        Invoke("DestroyAndReturnToPool", normalDestroyTime); 
    }

    public void Initialize(int damage)
    {
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<IDamageable>()?.Damage(damage);
            //Debug.Log("Hit");

            Instantiate(hitParticleSystem, transform.position, Quaternion.identity);


            DestroyAndReturnToPool();
        }
    }

    public void DestroyAndReturnToPool()
    {
        CancelInvoke();
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
