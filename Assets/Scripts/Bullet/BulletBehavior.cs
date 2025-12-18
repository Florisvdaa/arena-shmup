using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [SerializeField] private float normalBulletSpeed = 15f;

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

        Invoke("Destroy", 1f);
    }

    public void Destroy()
    {
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }
}
