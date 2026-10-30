using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager main;

    [SerializeField] private TowerRegistry towerRegistry;

    private void Awake()
    {
        if (main != null) { Destroy(gameObject); return; }
        main = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsUpgradeUnlocked(string upgradeId)
    {
        return SaveManager.main.CurrentSave.purchasedUpgrades.Contains(upgradeId);
    }

    public bool CanPurchaseUpgrade(string towerId, string pathId, int tierIndex)
    {
        TowerDefinition tower = towerRegistry.GetById(towerId);
        if (tower == null) return false;

        UpgradePath path = tower.upgradePaths.Find(p => p.pathId == pathId);
        if (path == null || tierIndex >= path.tiers.Count) return false;

        UpgradeTier tier = path.tiers[tierIndex];

        // tier 0 is always available if not already purchased
        if (tierIndex == 0)
            return !SaveManager.main.CurrentSave.purchasedUpgrades.Contains(tier.upgradeId)
                && SaveManager.main.CurrentSave.essence >= tier.cost;

        // higher tiers require previous tier to be purchased
        string previousId = path.tiers[tierIndex - 1].upgradeId;
        if (!SaveManager.main.CurrentSave.purchasedUpgrades.Contains(previousId))
            return false;

        return !SaveManager.main.CurrentSave.purchasedUpgrades.Contains(tier.upgradeId)
            && SaveManager.main.CurrentSave.essence >= tier.cost;
    }

    public bool PurchaseUpgrade(string towerId, string pathId, int tierIndex)
    {
        if (!CanPurchaseUpgrade(towerId, pathId, tierIndex)) return false;

        TowerDefinition tower = towerRegistry.GetById(towerId);
        UpgradePath path = tower.upgradePaths.Find(p => p.pathId == pathId);
        UpgradeTier tier = path.tiers[tierIndex];

        SaveManager.main.CurrentSave.essence -= tier.cost;
        SaveManager.main.CurrentSave.purchasedUpgrades.Add(tier.upgradeId);
        SaveManager.main.SaveCurrentSlot();
        return true;
    }
}