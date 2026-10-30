using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelUpgradeManager : MonoBehaviour
{
    public static LevelUpgradeManager main;

    public UnityEvent onUpgradesChanged = new UnityEvent();

    private Dictionary<string, int> tiers = new Dictionary<string, int>();

    private void Awake()
    {
        main = this; // a fresh instance per level, so tiers reset automatically
    }

    public int GetTier(string towerId)
    {
        return tiers.TryGetValue(towerId, out int t) ? t : 0;
    }

    public UpgradeTier GetNextTier(LevelLoadout.Entry entry)
    {
        UpgradePath path = entry.tower.upgradePaths[entry.pathIndex];
        int tier = GetTier(entry.tower.towerId);
        return tier < path.tiers.Count ? path.tiers[tier] : null;
    }

    public bool IsUnlockedInHub(UpgradeTier tier)
    {
        SaveData save = SaveManager.main != null ? SaveManager.main.CurrentSave : null;
        return save != null && save.purchasedUpgrades.Contains(tier.upgradeId);
    }

    public bool TryBuyNext(LevelLoadout.Entry entry)
    {
        UpgradeTier next = GetNextTier(entry);
        if (next == null) return false;                 // maxed
        if (!IsUnlockedInHub(next)) return false;       // not unlocked with Essence yet
        if (!LevelManager.main.SpendEcto(next.ectoCost)) return false;

        tiers[entry.tower.towerId] = GetTier(entry.tower.towerId) + 1;
        onUpgradesChanged.Invoke();
        return true;
    }
}