using UnityEngine;
using UnityEngine.SceneManagement;

public class HubManager : MonoBehaviour
{
    public static HubManager main;

    [Header("Panels")]
    [SerializeField] private GameObject hubPanel;
    [SerializeField] private GameObject worldSelectPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject loadoutPanel;
    [SerializeField] private GameObject ghostPanel;

    [Header("References")]
    [SerializeField] private LevelRegistry levelRegistry;
    [SerializeField] private ConstellationMapUI constellationMapUI;
    [SerializeField] private LoadoutUI loadoutUI;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private LevelDefinition pendingLevel;

    private void Awake()
    {
        if (main != null) { Destroy(gameObject); return; }
        main = this;
    }

    private void Start()
    {
    #if UNITY_EDITOR
        if (SaveManager.main.CurrentSave == null)
            SaveManager.main.LoadSlot(0); // only when testing straight from the hub
    #endif
        ShowHub();
    }

    // --- Hub ---
    public void ShowHub()
    {
        hubPanel.SetActive(true);
        worldSelectPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        loadoutPanel.SetActive(false);
        ghostPanel.SetActive(false);
    }

    public void OnMainMenuPressed()
    {
        SaveManager.main.SaveCurrentSlot();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // --- Ghosts ---
    public void OnGhostsPressed()
    {
        hubPanel.SetActive(false);
        ghostPanel.SetActive(true);
    }

    public void OnBackFromGhosts()
    {
        ghostPanel.SetActive(false);
        hubPanel.SetActive(true);
    }

    // --- Level Select ---
    public void OnLevelSelectPressed()
    {
        hubPanel.SetActive(false);
        worldSelectPanel.SetActive(true);
    }

    public void OnWorldSelected(int worldIndex)
    {
        if (!levelRegistry.IsWorldUnlocked(worldIndex, SaveManager.main.CurrentSave))
            return;

        worldSelectPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        constellationMapUI.LoadWorld(worldIndex);
    }

    public void OnLevelSelected(LevelDefinition level)
    {
        int globalIndex = levelRegistry.GetGlobalLevelIndex(level.levelId);
        if (globalIndex > SaveManager.main.CurrentSave.highestLevelUnlocked)
            return;

        pendingLevel = level;
        levelSelectPanel.SetActive(false);
        loadoutPanel.SetActive(true);
        loadoutUI.Setup(level);
    }

    public void OnEnterLevel()
    {
        loadoutUI.CommitLoadout();
        SceneManager.LoadScene(pendingLevel.sceneName);
        SaveData save = SaveManager.main.CurrentSave;
        int needed = Mathf.Min(pendingLevel.towerSlotCount, save.unlockedTowers.Count);
        if (save.selectedTowers.Count != needed) return;
        SceneManager.LoadScene(pendingLevel.sceneName);
    }

    public void OnBackFromWorldSelect()
    {
        worldSelectPanel.SetActive(false);
        hubPanel.SetActive(true);
    }

    public void OnBackFromLevelSelect()
    {
        levelSelectPanel.SetActive(false);
        worldSelectPanel.SetActive(true);
    }

    public void OnBackFromLoadout()
    {
        loadoutPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }
}