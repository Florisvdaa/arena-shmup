using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the pixel charging effect by spawning cubes, stacking them into a block,
/// and notifying the controller when charging completes.
/// </summary>
public class PixelChargeInEffect : MonoBehaviour
{
    #region Inspector Fields
    [Header("Pixel Charge Settings")]
    [Tooltip("Prefab for individual pixel cubes.")]
    [SerializeField] private GameObject pixelCubePrefab;
    [Tooltip("Radius around the target to spawn cubes initially.")]
    [SerializeField] private float spawnRadius = 2f;
    [Tooltip("Speed at which cubes move into stack positions.")]
    [SerializeField] private float pullSpeed = 5f;
    [Tooltip("Time to wait before spawning the enemy after cubes stack.")]
    [SerializeField] private float delayBeforeSpawn = 2f;

    [Header("Stack Layout")]
    [Tooltip("Vertical spacing between cube layers.")]
    [SerializeField] private float stackVerticalSpacing = 0.5f;
    [Tooltip("Horizontal spacing between cubes in each layer.")]
    [SerializeField] private float stackHorizontalSpacing = 0.5f;
    #endregion

    #region Private Fields
    private GameObject[] cubes;
    private Vector3[] cubeStackPositions;
    private Vector3 targetPosition;
    private bool charging;
    private float timer;
    private PixelEffectController controller;
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize cube arrays and calculate stack positions.
    /// </summary>
    private void Start()
    {
        targetPosition = transform.position;
        int cubeCount = 8; // 4 bottom + 4 top
        cubes = new GameObject[cubeCount];
        cubeStackPositions = new Vector3[cubeCount];

        // Compute stack positions for two layers (2×2 grid)
        for (int i = 0; i < cubeCount; i++)
        {
            int layer = (i < 4) ? 0 : 1;
            int index = i % 4;
            int xIdx = index % 2;
            int zIdx = index / 2;

            float xOff = (xIdx - 0.5f) * stackHorizontalSpacing;
            float yOff = layer * stackVerticalSpacing;
            float zOff = (zIdx - 0.5f) * stackHorizontalSpacing;

            cubeStackPositions[i] = targetPosition + new Vector3(xOff, yOff, zOff);
        }

        // Spawn cubes around effect
        for (int i = 0; i < cubeCount; i++)
        {
            Vector3 spawnPos = targetPosition + Random.onUnitSphere * spawnRadius;
            cubes[i] = Instantiate(pixelCubePrefab, spawnPos, Random.rotation, transform);
            if (cubes[i].TryGetComponent<Rigidbody>(out var rb))
                rb.useGravity = false;
        }
    }

    /// <summary>
    /// Update loop moves cubes toward stack and triggers spawn when timer elapses.
    /// </summary>
    private void Update()
    {
        if (!charging)
            return;

        timer += Time.deltaTime;

        // Move cubes into their designated positions
        for (int i = 0; i < cubes.Length; i++)
        {
            var cube = cubes[i];
            if (cube != null)
            {
                cube.transform.position = Vector3.MoveTowards(
                    cube.transform.position,
                    cubeStackPositions[i],
                    pullSpeed * Time.deltaTime);
            }
        }

        // When delay reached, notify controller and clean up
        if (timer >= delayBeforeSpawn)
        {
            charging = false;
            controller.TriggerEnemySpawn();

            foreach (var cube in cubes)
                if (cube != null)
                    Destroy(cube);

            Destroy(gameObject);
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Begins the charging process and resets the timer.
    /// </summary>
    /// <param name="ctrl">Reference to the owning PixelEffectController.</param>
    public void StartCharging(PixelEffectController ctrl)
    {
        controller = ctrl;
        timer = 0f;
        charging = true;
    }
    #endregion
}
