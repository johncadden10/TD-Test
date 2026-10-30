using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldSelectUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelRegistry levelRegistry;
    [SerializeField] private Image paintingImage;
    [SerializeField] private TextMeshProUGUI worldNameText;
    [SerializeField] private TextMeshProUGUI levelProgressText;
    [SerializeField] private GameObject worldInfoObject; // parent of name and progress text
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private Button paintingButton;
    [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color unlockedColor = Color.white;

    private int currentWorldIndex = 0;
    private int totalWorlds;

    private void Start()
    {
        totalWorlds = levelRegistry.worlds.Count;
        RefreshDisplay();
    }

    public void OnLeftArrowPressed()
    {
        currentWorldIndex = Mathf.Max(0, currentWorldIndex - 1);
        RefreshDisplay();
    }

    public void OnRightArrowPressed()
    {
        currentWorldIndex = Mathf.Min(totalWorlds - 1, currentWorldIndex + 1);
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        leftArrowButton.interactable = currentWorldIndex > 0;
        rightArrowButton.interactable = currentWorldIndex < totalWorlds - 1;

        WorldDefinition world = levelRegistry.worlds[currentWorldIndex];
        SaveData save = SaveManager.main.CurrentSave;
        bool isUnlocked = levelRegistry.IsWorldUnlocked(currentWorldIndex, save);

        // painting
        paintingImage.sprite = world.background;
        paintingImage.color = isUnlocked ? unlockedColor : lockedColor;
        paintingButton.interactable = isUnlocked;

        // info
        worldInfoObject.SetActive(isUnlocked);

        if (isUnlocked)
        {
            worldNameText.text = world.worldName;
            levelProgressText.text = $"{GetCompletedLevels(world, save)}/{world.levels.Count} Levels";
        }
    }

    private int GetCompletedLevels(WorldDefinition world, SaveData save)
    {
        int completed = 0;
        int globalIndex = 0;

        foreach (WorldDefinition w in levelRegistry.worlds)
        {
            foreach (LevelDefinition level in w.levels)
            {
                if (w == world && globalIndex < save.highestLevelUnlocked)
                    completed++;
                globalIndex++;
            }
        }

        return completed;
    }

    public void OnPaintingPressed()
    {
        HubManager.main.OnWorldSelected(currentWorldIndex);
    }
}