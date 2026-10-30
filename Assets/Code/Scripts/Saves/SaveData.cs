using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public string saveName = "New Save";
    public int essence = 0;
    public int highestLevelUnlocked = 0;
    public List<string> unlockedTowers = new List<string>() { "phantom" }; // phantom unlocked by default
    public List<string> purchasedUpgrades = new List<string>(); // e.g. "phantom_path1_tier1"
    public List<string> selectedTowers = new List<string>(); // 4 unique tower IDs
    public string selectedUpgradePath = ""; // "path1" or "path2"
}