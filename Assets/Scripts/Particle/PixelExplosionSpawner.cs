using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Handles spawning a random number of pixel cubes and applying explosion physics.
/// </summary>
public class PixelExplosionSpawner : MonoBehaviour
{
    #region Inspector Fields
    [Tooltip("Prefab used for each pixel cube spawned during explosion.")]
    [SerializeField] private GameObject pixelCubePrefab;
    [Tooltip("Minimum number of cubes spawned when exploding.")]
    [SerializeField] private int minCubes = 3;
    [Tooltip("Maximum number of cubes spawned when exploding.")]
    [SerializeField] private int maxCubes = 10;
    [Tooltip("Force of the explosion applied to each cube.")]
    [SerializeField] private float explosionForce = 5f;
    [Tooltip("Radius of effect for the explosion force.")]
    [SerializeField] private float explosionRadius = 2f;
    [Tooltip("Upward modifier applied to explosion force for vertical lift.")]
    [SerializeField] private float upwardModifier = 1f;
    #endregion

    #region Public Methods
    /// <summary>
    /// Spawns a random number of pixel cubes at this transform's position,
    /// applies an explosion force to each, and schedules their destruction.
    /// </summary>
    public void Explode()
    {
        // Determine how many cubes to spawn
        int cubeCount = Random.Range(minCubes, maxCubes + 1);

        // Spawn cubes and apply physics
        for (int i = 0; i < cubeCount; i++)
        {
            // Instantiate cube with random rotation
            GameObject cube = Instantiate(pixelCubePrefab, transform.position, Random.rotation);

            // Apply explosion force if Rigidbody exists
            if (cube.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddExplosionForce(
                    explosionForce,
                    transform.position,
                    explosionRadius,
                    upwardModifier,
                    ForceMode.Impulse
                );
            }

            // Destroy cube after 2 seconds to clean up
            Destroy(cube, 2f);
        }
    }
    #endregion
}
