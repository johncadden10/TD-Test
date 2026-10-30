using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UpgradeTier
{
    public string upgradeId;    // e.g. "phantom_path1_tier1"
    public string displayName;
    public string description;
    public int cost;
    public Sprite icon;
}

[System.Serializable]
public class UpgradePath
{
    public string pathId;       // "path1" or "path2"
    public string displayName;
    public List<UpgradeTier> tiers; // always 3
}

[System.Serializable]
public class TowerDefinition
{
    public string towerId;
    public string displayName;
    public string description;
    public int cost;
    public float baseRange;
    public float baseAttackSpeed;
    public int baseDamage;
    public GameObject prefab;
    public Sprite icon;
    public bool unlockedByDefault;
    public int unlockedAfterLevel;  // level index that unlocks this tower
    public List<UpgradePath> upgradePaths; // always 2 paths
}