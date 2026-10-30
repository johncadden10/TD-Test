using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GhostDetailUI : MonoBehaviour
{
    [Header("Tower Info References")]
    [SerializeField] private Image towerIcon;
    [SerializeField] private TextMeshProUGUI towerNameText;
    [SerializeField] private TextMeshProUGUI towerDescriptionText;
    [SerializeField] private TextMeshProUGUI towerCostText;
    [SerializeField] private TextMeshProUGUI towerStatsText;

    [Header("Upgrade References")]
    [SerializeField] private GhostUpgradePathUI path1UI;
    [SerializeField] private GhostUpgradePathUI path2UI;
    [SerializeField] private GhostUpgradeInfoPanelUI upgradeInfoPanel;

    private TowerDefinition towerData;
    private GhostGridUI gridUI;

    public void Setup(TowerDefinition tower, GhostGridUI grid)
    {
        towerData = tower;
        gridUI = grid;

        towerIcon.sprite = tower.icon;
        towerNameText.text = tower.displayName;
        towerDescriptionText.text = tower.description;
        towerCostText.text = $"Cost: {tower.cost} Ecto";
        towerStatsText.text = BuildStatsText(tower);

        RefreshUpgradePaths();
        upgradeInfoPanel.gameObject.SetActive(false);
    }

    public void RefreshUpgradePaths()
    {
        path1UI.Setup(towerData, towerData.upgradePaths[0], upgradeInfoPanel, this);
        path2UI.Setup(towerData, towerData.upgradePaths[1], upgradeInfoPanel, this);
    }

    public void OnClosePressed()
    {
        gameObject.SetActive(false);
        gridUI.ShowGrid();
    }

    private string BuildStatsText(TowerDefinition tower)
    {
        return $"Range: {tower.baseRange}\nAttack Speed: {tower.baseAttackSpeed}\nDamage: {tower.baseDamage}";
    }
}