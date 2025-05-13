#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    static void OpenWindow()
    {
        GetWindow<FindMissingScripts>("Find Missing Scripts");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Scan Scene for Missing Scripts"))
            ScanForMissing();
    }

    static void ScanForMissing()
    {
        int missingCount = 0;
        foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            // skip prefab assets in project view
            if (EditorUtility.IsPersistent(go))
                continue;

            var components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    missingCount++;
                    Debug.LogError(
                        $"❗ Missing script on [{GetFullPath(go)}] at slot #{i}",
                        go
                    );
                }
            }
        }

        if (missingCount == 0)
            Debug.Log("✅ No missing scripts found in the active scenes.");
        else
            Debug.LogError($"⚠️ Found {missingCount} missing script slot(s). Check the Console entries above.");
    }

    static string GetFullPath(GameObject go)
    {
        return go.transform.parent == null
            ? go.name
            : GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
    }
}
#endif
