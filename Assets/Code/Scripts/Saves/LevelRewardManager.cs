using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRewardManager : MonoBehaviour
{
    public static LevelRewardManager main;

    [SerializeField] private LevelRegistry levelRegistry;
    [SerializeField] private string hubSceneName = "Hub";

    private void Awake()
    {
        main = this;
    }

    /*
    public void CompleteLevel(string levelId)
    {
        SaveData save = SaveManager.main.CurrentSave;
        LevelDefinition level = levelRegistry.levels.Find(l => l.levelId == levelId);

        if (level == null) return;

        int levelIndex = levelRegistry.levels.IndexOf(level);

        // unlock next level
        if (levelIndex + 1 > save.highestLevelUnlocked)
            save.highestLevelUnlocked = levelIndex + 1;

        // grant currency
        save.essence += level.essenceReward;

        SaveManager.main.SaveCurrentSlot();
        SceneManager.LoadScene(hubSceneName);
    } */
}
