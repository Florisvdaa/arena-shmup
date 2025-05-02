using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelExplosionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pixelCubePrefab;
    [SerializeField] private int minCubes = 3;
    [SerializeField] private int maxCubes = 10;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float upwardModifier = 1f;

    public void Explode()
    {
        int cubeCount = Random.Range(minCubes, maxCubes + 1);

        for (int i = 0; i < cubeCount; i++)
        {
            GameObject cube = Instantiate(pixelCubePrefab, transform.position, Random.rotation);
            Rigidbody rb = cube.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 explosionPos = transform.position;
                rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, upwardModifier, ForceMode.Impulse);
            }

            Destroy(cube, 2f); // auto-clean after 2 seconds
        }
    }
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Explode();
    //    }
    //}
}
