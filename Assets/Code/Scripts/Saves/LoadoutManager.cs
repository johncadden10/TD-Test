using System.Collections.Generic;
using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    public static LoadoutManager main;

    private const int LOADOUT_SIZE = 4;

    private void Awake()
    {
        if (main != null) { Destroy(gameObject); return; }
        main = this;
        DontDestroyOnLoad(gameObject);
    }

    public List<string> GetSelectedTowers()
    {
        return SaveManager.main.CurrentSave.selectedTowers;
    }

    public bool SelectTower(string towerId)
    {
        var selected = SaveManager.main.CurrentSave.selectedTowers;

        if (selected.Contains(towerId))
        {
            selected.Remove(towerId);
            return false; // deselected
        }

        if (selected.Count >= LOADOUT_SIZE)
        {
            Debug.Log("Loadout full — deselect a tower first");
            return false;
        }

        selected.Add(towerId);
        return true; // selected
    }

    public void SelectUpgradePath(string pathId)
    {
        SaveManager.main.CurrentSave.selectedUpgradePath = pathId;
    }

    public bool IsLoadoutReady(int requiredSlots)
    {
        return SaveManager.main.CurrentSave.selectedTowers.Count == requiredSlots;
    }
}