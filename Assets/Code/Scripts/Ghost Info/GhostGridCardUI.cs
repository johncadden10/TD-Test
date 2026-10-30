using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GhostGridCardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image towerIcon;
    [SerializeField] private TextMeshProUGUI towerNameText;
    [SerializeField] private GameObject lockedOverlay;
    [SerializeField] private Button selectButton;

    private TowerDefinition towerData;
    private GhostGridUI gridUI;

    public void Setup(TowerDefinition tower, bool isUnlocked, GhostGridUI grid)
    {
        towerData = tower;
        gridUI = grid;

        towerIcon.sprite = tower.icon;
        towerIcon.color = isUnlocked ? Color.white : Color.grey;
        towerNameText.text = isUnlocked ? tower.displayName : "???";
        lockedOverlay.SetActive(!isUnlocked);
        selectButton.interactable = isUnlocked;
    }

    public void OnCardPressed()
    {
        gridUI.OnTowerSelected(towerData);
    }
}