#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Exports all sub-sprites from a sliced sprite sheet into individual PNG files.
/// Place this script under an "Editor" folder in your project.
/// </summary>
public class SpriteExporter : MonoBehaviour
{
    private const string exportPath = "Assets/ExportedSprites";

    [MenuItem("Assets/Export Sliced Sprites")]
    private static void ExportSelectedSlicedSprites()
    {
        // Ensure export directory exists
        if (!Directory.Exists(exportPath))
            Directory.CreateDirectory(exportPath);

        // Get selected texture
        var obj = Selection.activeObject as Texture2D;
        if (obj == null)
        {
            Debug.LogError("Please select a sliced sprite texture asset in the Project window.");
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(obj);
        var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);

        foreach (var spr in sprites)
        {
            if (spr is Sprite)
            {
                var sprite = spr as Sprite;
                string spriteName = sprite.name;

                // Create readable copy of the texture
                Texture2D tex = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
                var pixels = sprite.texture.GetPixels(
                    (int)sprite.textureRect.x,
                    (int)sprite.textureRect.y,
                    (int)sprite.textureRect.width,
                    (int)sprite.textureRect.height);
                tex.SetPixels(pixels);
                tex.Apply();

                // Encode to PNG
                byte[] png = tex.EncodeToPNG();
                DestroyImmediate(tex);

                // Write to file
                string filePath = Path.Combine(exportPath, spriteName + ".png");
                File.WriteAllBytes(filePath, png);

                Debug.Log($"Exported: {filePath}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Export complete.");
    }
}
#endif
