using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GhostUpgradePathUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI pathNameText;
    [SerializeField] private List<GhostUpgradeTierUI> tierButtons;

    public void Setup(TowerDefinition tower, UpgradePath path, GhostUpgradeInfoPanelUI infoPanel, GhostDetailUI detailUI)
    {
        pathNameText.text = path.displayName;

        SaveData save = SaveManager.main.CurrentSave;

        for (int i = 0; i < tierButtons.Count; i++)
        {
            if (i >= path.tiers.Count) continue;

            UpgradeTier tier = path.tiers[i];
            bool purchased = save.purchasedUpgrades.Contains(tier.upgradeId);
            bool available = UpgradeManager.main.CanPurchaseUpgrade(tower.towerId, path.pathId, i);

            tierButtons[i].Setup(tower.towerId, path.pathId, i, tier, purchased, available, infoPanel, detailUI);
        }
    }
}