using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple object pool managing reusable bullet instances to avoid runtime allocations.
/// </summary>
public class BulletPool : MonoBehaviour
{
    #region Singleton
    /// <summary>
    /// Global access point for the BulletPool instance.
    /// </summary>
    public static BulletPool Instance { get; private set; }
    #endregion

    #region Inspector Fields
    [Tooltip("Prefab used for bullet instances in the pool.")]
    [SerializeField] private GameObject bulletPrefab;

    [Tooltip("Initial number of bullets to create in the pool.")]
    [SerializeField] private int poolSize = 20;
    #endregion

    #region Private Fields
    // Queue to hold inactive bullet game objects.
    private Queue<GameObject> pool = new Queue<GameObject>();
    #endregion

    #region Unity Callbacks
    /// <summary>
    /// Initialize singleton and prepopulate the pool.
    /// </summary>
    private void Awake()
    {
        // Enforce singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Pre-instantiate bullet instances and disable them
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(false);
            pool.Enqueue(bullet);
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Retrieves an inactive bullet from the pool or logs a warning if exhausted.
    /// </summary>
    public GameObject GetBullet()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            Debug.LogWarning("Bullet Pool exhausted! Consider increasing pool size.");
            return null;
        }
    }

    /// <summary>
    /// Returns a bullet to the pool and deactivates it.
    /// </summary>
    /// <param name="bullet">The bullet GameObject to return.</param>
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        pool.Enqueue(bullet);
    }
    #endregion
}
