using System.Collections.Generic;
using UnityEngine;

public class TowerGridUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TowerRegistry towerRegistry;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject towerCardPrefab;

    private List<TowerCardUI> spawnedCards = new List<TowerCardUI>();

    public void Setup()
    {
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);
        spawnedCards.Clear();

        SaveData save = SaveManager.main.CurrentSave;

        foreach (TowerDefinition tower in towerRegistry.towers)
        {
            if (!save.unlockedTowers.Contains(tower.towerId))
                continue;

            GameObject cardObj = Instantiate(towerCardPrefab, gridContainer);
            TowerCardUI card = cardObj.GetComponent<TowerCardUI>();
            card.Setup(tower, this);
            spawnedCards.Add(card);
        }

        RefreshCards(); // refresh immediately after spawning
    }

    public void OnTowerSelected(TowerDefinition tower)
    {
        LoadoutUI.main.OnTowerSelectedFromGrid(tower);
    }

    public void RefreshCards()
    {
        SaveData save = SaveManager.main.CurrentSave;
        foreach (TowerCardUI card in spawnedCards)
            card.RefreshState(save.selectedTowers.Contains(card.TowerId));
    }
}