using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FolderColorizer
{
    // Map folder names to specific tint colors
    private static readonly Dictionary<string, Color> FolderColors = new Dictionary<string, Color>
    {
        { "Scripts",    new Color(0.2f, 0.8f, 0.4f, 0.25f) },
        { "Prefabs",    new Color(0.2f, 0.6f, 1.0f, 0.25f) },
        { "Scenes",     new Color(1.0f, 0.4f, 0.4f, 0.25f) },
        { "Registries", new Color(0.9f, 0.6f, 0.2f, 0.25f) },
        { "Levels",     new Color(0.2f, 0.7f, 0.7f, 0.25f) },
        { "Stars",     new Color(0.9f, 0.9f, 0.9f, 0.25f) },
        { "Sprites",    new Color(0.9f, 0.7f, 0.3f, 0.25f) }
    };

    static FolderColorizer()
    {
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
    }

    private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (!AssetDatabase.IsValidFolder(path)) return;

        string folderName = System.IO.Path.GetFileName(path);
        if (FolderColors.TryGetValue(folderName, out Color tintColor))
        {
            EditorGUI.DrawRect(selectionRect, tintColor);
        }
    }
}
