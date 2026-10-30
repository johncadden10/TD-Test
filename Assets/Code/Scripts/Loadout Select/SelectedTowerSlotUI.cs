using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectedTowerSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject emptySlotObject;
    [SerializeField] private GameObject filledSlotObject;
    [SerializeField] private Image towerIcon;
    [SerializeField] private TextMeshProUGUI towerNameText;
    [SerializeField] private UpgradePathUI upgradePathUI;
    [SerializeField] private Button pathToggleButton;
    [SerializeField] private Color selectedPathColor = Color.cyan;
    [SerializeField] private Color unselectedPathColor = Color.grey;

    private TowerDefinition towerData;
    private string selectedPath = "path1"; // default to path1

    public string TowerId => towerData?.towerId;
    public bool IsEmpty => towerData == null;
    public string SelectedPath => selectedPath;
    public TowerDefinition Tower => towerData;
    public int PathIndex => selectedPath == "path1" ? 0 : 1;

    public void SetTower(TowerDefinition tower)
    {
        towerData = tower;
        selectedPath = "path1"; // reset to path1 when new tower selected

        emptySlotObject.SetActive(false);
        filledSlotObject.SetActive(true);

        towerIcon.sprite = tower.icon;
        towerNameText.text = tower.displayName;

        upgradePathUI.Setup(tower, selectedPath);
        RefreshPathButton();
    }

    public void Clear()
    {
        towerData = null;
        emptySlotObject.SetActive(true);
        filledSlotObject.SetActive(false);
        upgradePathUI.Clear();
    }

    public void OnPathTogglePressed()
    {
        selectedPath = selectedPath == "path1" ? "path2" : "path1";
        upgradePathUI.Setup(towerData, selectedPath);
        RefreshPathButton();
    }

    private void RefreshPathButton()
    {
        pathToggleButton.GetComponentInChildren<TextMeshProUGUI>().text =
            selectedPath == "path1" ? "Path 1" : "Path 2";
    }

    public void OnSlotPressed()
    {
        if (towerData == null) return;
        LoadoutUI.main.OnTowerRemovedFromSlot(this);
    }
}