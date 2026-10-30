using UnityEngine;
using UnityEngine.UI;

public class ConstellationNode : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image nodeImage;
    [SerializeField] private Sprite completedSprite;
    [SerializeField] private Sprite uncompletedSprite;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private Color completedColor = Color.cyan;

    private LevelDefinition levelData;
    private bool isCompleted;

    public LevelDefinition LevelData => levelData;

    public void Setup(LevelDefinition level, bool completed)
    {
        levelData = level;
        isCompleted = completed;

        nodeImage.sprite = completed ? completedSprite : uncompletedSprite;
        nodeImage.color = completed ? completedColor : defaultColor;
    }

    public void SetSelected(bool selected)
    {
        if (isCompleted)
            nodeImage.color = selected ? selectedColor : completedColor;
        else
            nodeImage.color = selected ? selectedColor : defaultColor;
    }

    public void OnNodePressed()
    {
        ConstellationMapUI.main.OnNodeSelected(this);
    }
}