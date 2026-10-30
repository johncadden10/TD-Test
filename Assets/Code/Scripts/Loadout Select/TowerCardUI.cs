using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerCardUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image towerIcon;
    [SerializeField] private TextMeshProUGUI towerNameText;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color selectedColor = Color.grey;

    private TowerDefinition towerData;
    private TowerGridUI gridUI;

    public string TowerId => towerData.towerId;

    public void Setup(TowerDefinition tower, TowerGridUI grid)
    {
        towerData = tower;
        gridUI = grid;
        towerIcon.sprite = tower.icon;
        towerNameText.text = tower.displayName;
    }

    public void RefreshState(bool isSelected)
    {
        towerIcon.color = isSelected ? selectedColor : defaultColor;
        GetComponent<UnityEngine.UI.Button>().interactable = !isSelected;
    }

    public void OnCardPressed()
    {
        gridUI.OnTowerSelected(towerData);
    }
}