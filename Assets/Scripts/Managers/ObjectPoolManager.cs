using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> objectPools = new List<PooledObjectInfo>();

    private static GameObject objectHolder;

    public static void PrewarmPool(GameObject prefab, int count)
    {
        if (objectHolder == null)
            objectHolder = new GameObject("Pooled Objects");

        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == prefab.name);
        if (pool == null)
        {
            pool = new PooledObjectInfo() { lookupString = prefab.name };
            objectPools.Add(pool);
        }

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            go.name = prefab.name;
            go.transform.SetParent(objectHolder.transform);
            go.SetActive(false);
            pool.inactiveObjects.Add(go);
        }
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == objectToSpawn.name);

        // If Pool does not exist, create it.
        if (pool == null)
        {
            pool = new PooledObjectInfo() { lookupString = objectToSpawn.name };
            objectPools.Add(pool);
        }

        // Check if there are any inactive objects in the pool
        GameObject spawnableObj = pool.inactiveObjects.FirstOrDefault();

        if (spawnableObj == null)
        {
            // If there are no inactive objects, create a new one
            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);

            spawnableObj.transform.SetParent(objectHolder.transform);
        }
        else
        {
            // If there is an inactive object, reactivate it.
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.inactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }
    public static void ReturnObjectToPool(GameObject obj)
    {
        //string goName = obj.name.Substring(0, obj.name.Length - 7); // By taking off 7, we are removing the "(clone)" from the name of the passed in obj

        PooledObjectInfo pool = objectPools.Find(p => p.lookupString == obj.name);

        if (pool == null)
            Debug.LogWarning($"Trying to release an object that is not pooled {obj.name}");
        else
        {
            obj.SetActive(false);
            pool.inactiveObjects.Add(obj);
        }
    }    
}

public class PooledObjectInfo
{
    public string lookupString;
    public List<GameObject> inactiveObjects = new List<GameObject>();
}
