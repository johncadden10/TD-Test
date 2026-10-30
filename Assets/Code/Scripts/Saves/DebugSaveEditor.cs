#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class DebugSaveEditor : EditorWindow
{
    [MenuItem("Debug/Save Editor")]
    public static void ShowWindow()
    {
        GetWindow<DebugSaveEditor>("Save Editor");
    }

    private int essence;
    private int highestLevel;
    private string towerId;
    private string upgradeId;

    private void OnGUI()
    {
        GUILayout.Label("Save Debug Tools", EditorStyles.boldLabel);

        if (SaveManager.main == null || SaveManager.main.CurrentSave == null)
        {
            EditorGUILayout.HelpBox("No active save loaded. Enter Play Mode and load a slot first.", MessageType.Warning);
            return;
        }

        SaveData save = SaveManager.main.CurrentSave;

        EditorGUILayout.Space();
        GUILayout.Label("Essence", EditorStyles.boldLabel);
        GUILayout.Label($"Current Essence: {save.essence}"); // ← show current value
        essence = EditorGUILayout.IntField("New Essence Value", essence); // ← type new value here
        if (GUILayout.Button("Apply Essence"))
        {
            save.essence = essence;
            SaveManager.main.SaveCurrentSlot();
            Debug.Log($"Essence set to {save.essence}");
        }

        EditorGUILayout.Space();
        GUILayout.Label("Levels", EditorStyles.boldLabel);
        highestLevel = EditorGUILayout.IntField("Highest Level Unlocked", save.highestLevelUnlocked);
        if (GUILayout.Button("Apply Level Progress"))
        {
            save.highestLevelUnlocked = highestLevel;
            SaveManager.main.SaveCurrentSlot();
        }
        if (GUILayout.Button("Unlock All Levels"))
        {
            save.highestLevelUnlocked = 999;
            SaveManager.main.SaveCurrentSlot();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Towers", EditorStyles.boldLabel);
        towerId = EditorGUILayout.TextField("Tower ID", towerId);
        if (GUILayout.Button("Unlock Tower"))
        {
            if (!save.unlockedTowers.Contains(towerId))
            {
                save.unlockedTowers.Add(towerId);
                SaveManager.main.SaveCurrentSlot();
            }
        }
        if (GUILayout.Button("Unlock All Towers"))
        {
            // add all tower IDs here manually or pull from TowerRegistry
            foreach (string id in new[] { "vanish", "poltergeist", "banshee", "spirit", "wisp", "reaper", "phantom" })
            {
                if (!save.unlockedTowers.Contains(id))
                    save.unlockedTowers.Add(id);
            }
            SaveManager.main.SaveCurrentSlot();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Upgrades", EditorStyles.boldLabel);
        upgradeId = EditorGUILayout.TextField("Upgrade ID", upgradeId);
        if (GUILayout.Button("Grant Upgrade"))
        {
            if (!save.purchasedUpgrades.Contains(upgradeId))
            {
                save.purchasedUpgrades.Add(upgradeId);
                SaveManager.main.SaveCurrentSlot();
            }
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Save Now"))
            SaveManager.main.SaveCurrentSlot();
    }
}
#endif