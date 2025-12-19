using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float normalBulletSpeed = 15f;

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

        Invoke("DestroyAndReturnToPool", 1f);
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
            DestroyAndReturnToPool();
        }
    }

    public void DestroyAndReturnToPool()
    {
        CancelInvoke();
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
