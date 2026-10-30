using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutUI : MonoBehaviour
{
    public static LoadoutUI main;

    [Header("References")]
    [SerializeField] private TowerRegistry towerRegistry;
    [SerializeField] private EnemyPreviewUI enemyPreviewUI;
    [SerializeField] private TowerGridUI towerGridUI;
    [SerializeField] private List<SelectedTowerSlotUI> towerSlots;
    [SerializeField] private TextMeshProUGUI essenceText;
    [SerializeField] private Button enterLevelButton;
    [SerializeField] private TextMeshProUGUI selectedPathText;

    private LevelDefinition currentLevel;

    private void Awake()
    {
        main = this;
    }

    private int GetSlotLimit()
    {
        int unlocked = SaveManager.main.CurrentSave.unlockedTowers.Count;
        return Mathf.Min(currentLevel.towerSlotCount, unlocked);
    }

    public void Setup(LevelDefinition level)
    {
        currentLevel = level;
        SaveManager.main.CurrentSave.selectedTowers.Clear();

        int limit = GetSlotLimit();
        for (int i = 0; i < towerSlots.Count; i++)
        {
            bool active = i < limit;
            towerSlots[i].gameObject.SetActive(active);
            if (active) towerSlots[i].Clear();
        }

        enemyPreviewUI.Setup(level);
        towerGridUI.Setup();
        RefreshEssence();
        CheckLoadoutReady();
    }

    public void OnTowerSelectedFromGrid(TowerDefinition tower)
    {
        List<string> selected = SaveManager.main.CurrentSave.selectedTowers;

        if (selected.Contains(tower.towerId)) return;
        if (selected.Count >= GetSlotLimit()) return; // no free slot

        SelectedTowerSlotUI emptySlot = towerSlots.Find(s => s.gameObject.activeSelf && s.IsEmpty);
        if (emptySlot == null) return;

        selected.Add(tower.towerId);
        emptySlot.SetTower(tower);
        towerGridUI.RefreshCards();
        CheckLoadoutReady();
    }

    public void OnTowerRemovedFromSlot(SelectedTowerSlotUI slot)
    {
        if (slot.IsEmpty) return;

        SaveManager.main.CurrentSave.selectedTowers.Remove(slot.TowerId);
        slot.Clear();
        towerGridUI.RefreshCards();
        CheckLoadoutReady();
    }

    private void RefreshSlots()
    {
        foreach (SelectedTowerSlotUI slot in towerSlots)
            slot.Clear();

        List<string> selected = SaveManager.main.CurrentSave.selectedTowers;
        for (int i = 0; i < selected.Count && i < towerSlots.Count; i++)
        {
            TowerDefinition def = towerRegistry.GetById(selected[i]);
            if (def != null) towerSlots[i].SetTower(def);
        }
    }

    private void RefreshEssence()
    {
        essenceText.text = $"Essence: {SaveManager.main.CurrentSave.essence}";
    }

    private void CheckLoadoutReady()
    {
        enterLevelButton.interactable = SaveManager.main.CurrentSave.selectedTowers.Count == GetSlotLimit();
    }

    private TowerDefinition FindTowerById(string id)
    {
        return towerRegistry.GetById(id);
    }
    
    public void CommitLoadout()
    {
        LevelLoadout.entries.Clear();

        foreach (SelectedTowerSlotUI slot in towerSlots)
        {
            if (!slot.gameObject.activeSelf || slot.IsEmpty) continue;

            LevelLoadout.entries.Add(new LevelLoadout.Entry
            {
                tower = slot.Tower,
                pathIndex = slot.PathIndex
            });
        }
    }
}