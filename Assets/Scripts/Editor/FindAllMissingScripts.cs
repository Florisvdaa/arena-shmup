#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FindAllMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find All Missing Scripts")]
    public static void OpenWindow()
    {
        GetWindow<FindAllMissingScripts>("Find Missing Scripts");
    }

    void OnGUI()
    {
        if (GUILayout.Button("1) Scan Scene"))
            ScanScene();
        if (GUILayout.Button("2) Scan Prefab Assets"))
            ScanPrefabs();
    }

    static void ScanScene()
    {
        int missing = 0;
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            // only scene objects
            if (EditorUtility.IsPersistent(go)) continue;

            missing += CheckGameObject(go);
        }
        Debug.Log(missing == 0
            ? "✅ No missing scripts in scene objects."
            : $"⚠️ Found {missing} missing script slots in scene objects.");
    }

    static void ScanPrefabs()
    {
        int missing = 0;
        // find all prefabs in the asset database
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            // check the root plus all children
            var all = prefab.GetComponentsInChildren<Transform>(true);
            foreach (var t in all)
                missing += CheckGameObject(t.gameObject, path);
        }

        Debug.Log(missing == 0
            ? "✅ No missing scripts in prefabs."
            : $"⚠️ Found {missing} missing script slots in prefab assets (see Console).");
    }

    static int CheckGameObject(GameObject go, string context = null)
    {
        var comps = go.GetComponents<Component>();
        int found = 0;
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i] == null)
            {
                found++;
                string prefix = context != null
                    ? $"[Prefab: {context}] "
                    : "";
                Debug.LogError(
                    $"{prefix}Missing script on GameObject '{GetFullPath(go)}' (slot #{i})",
                    go
                );
            }
        }
        return found;
    }

    static string GetFullPath(GameObject go)
    {
        return go.transform.parent == null
            ? go.name
            : GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
    }
}
#endif
