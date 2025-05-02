using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollisionCircleEffect : MonoBehaviour
{
    public GameObject circleEffectPrefab;
    public float destroyAfterSeconds = 2f;

    private void OnCollisionEnter(Collision collision)
    {
        if (circleEffectPrefab == null || collision.contactCount == 0)
            return;

        ContactPoint contact = collision.GetContact(0); // slightly faster than .contacts[0]

        // Get the rotation that aligns the particle with the wall surface
        // We use Quaternion.LookRotation(normal) to face "out" from wall
        // Then rotate it to lay flat using Quaternion.LookRotation + Quaternion.AngleAxis
        Quaternion rotation = Quaternion.LookRotation(contact.normal);

        // Slight offset to avoid z-fighting
        Vector3 spawnPosition = contact.point + contact.normal * 0.01f;

        GameObject particle = Instantiate(circleEffectPrefab, spawnPosition, rotation, transform);

        Destroy(particle, destroyAfterSeconds);
    }
}
