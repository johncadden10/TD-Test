using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRewardManager : MonoBehaviour
{
    public static LevelRewardManager main;

    [SerializeField] private LevelRegistry levelRegistry;
    [SerializeField] private TowerRegistry towerRegistry;
    [SerializeField] private string hubSceneName = "HubScene";

    public static readonly System.Collections.Generic.List<string> newlyUnlocked = new System.Collections.Generic.List<string>();

    private void Awake()
    {
        main = this;
    }

    public void CompleteLevel(string levelId)
    {
        SaveData save = SaveManager.main.CurrentSave;
        LevelDefinition level = levelRegistry.GetLevelById(levelId);
        if (level == null) return;

        int levelIndex = levelRegistry.GetGlobalLevelIndex(levelId);

        // unlock next level
        if (levelIndex + 1 > save.highestLevelUnlocked)
            save.highestLevelUnlocked = levelIndex + 1;

        save.essence += level.essenceReward;

        // unlock ghosts tied to this level
        newlyUnlocked.Clear();
        foreach (TowerDefinition t in towerRegistry.towers)
        {
            if (t.unlockedByDefault) continue;
            if (t.unlockedAfterLevel > levelIndex) continue;   // not reached yet
            if (save.unlockedTowers.Contains(t.towerId)) continue;

            save.unlockedTowers.Add(t.towerId);
            newlyUnlocked.Add(t.displayName);
        }

        SaveManager.main.SaveCurrentSlot();
        SceneManager.LoadScene(hubSceneName);
    }
}
