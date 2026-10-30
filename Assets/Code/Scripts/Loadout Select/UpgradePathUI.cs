using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePathUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject upgradePathObject;
    [SerializeField] private List<UpgradeTierButtonUI> path1Buttons;
    [SerializeField] private List<UpgradeTierButtonUI> path2Buttons;
    [SerializeField] private GameObject upgradeInfoPanel;
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;

    private TowerDefinition towerData;

    public void Setup(TowerDefinition tower, string selectedPath)
    {
        towerData = tower;
        upgradePathObject.SetActive(true);
        upgradeInfoPanel.SetActive(false);

        SaveData save = SaveManager.main.CurrentSave;

        // show both paths but highlight selected one
        SetupPathButtons(path1Buttons, tower.upgradePaths[0], save, selectedPath == "path1");
        SetupPathButtons(path2Buttons, tower.upgradePaths[1], save, selectedPath == "path2");
    }

    public void Clear()
    {
        towerData = null;
        upgradePathObject.SetActive(false);
        upgradeInfoPanel.SetActive(false);
    }

    private void SetupPathButtons(List<UpgradeTierButtonUI> buttons, UpgradePath path, SaveData save, bool isSelectedPath)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i >= path.tiers.Count) continue;
            UpgradeTier tier = path.tiers[i];
            bool purchased = save.purchasedUpgrades.Contains(tier.upgradeId);
            bool available = UpgradeManager.main.CanPurchaseUpgrade(towerData.towerId, path.pathId, i);
            buttons[i].Setup(tier, purchased, available, isSelectedPath, this);
        }
    }

    public void ShowUpgradeInfo(UpgradeTier tier)
    {
        upgradeInfoPanel.SetActive(true);
        upgradeNameText.text = tier.displayName;
        upgradeDescriptionText.text = tier.description;
        upgradeCostText.text = $"Cost: {tier.cost} Essence";
    }
}