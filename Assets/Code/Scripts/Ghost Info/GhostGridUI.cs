using System.Collections.Generic;
using UnityEngine;

public class GhostGridUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TowerRegistry towerRegistry;
    [SerializeField] private Transform gridContainer;
    [SerializeField] private GameObject ghostGridCardPrefab;
    [SerializeField] private GhostDetailUI ghostDetailUI;

    private void OnEnable()
    {
        RefreshGrid();
    }

    private void RefreshGrid()
    {
        foreach (Transform child in gridContainer)
            Destroy(child.gameObject);

        SaveData save = SaveManager.main.CurrentSave;

        foreach (TowerDefinition tower in towerRegistry.towers)
        {
            GameObject cardObj = Instantiate(ghostGridCardPrefab, gridContainer);
            GhostGridCardUI card = cardObj.GetComponent<GhostGridCardUI>();
            bool isUnlocked = save.unlockedTowers.Contains(tower.towerId);
            card.Setup(tower, isUnlocked, this);
        }
    }

    public void OnTowerSelected(TowerDefinition tower)
    {
        ghostDetailUI.Setup(tower, this);
        ghostDetailUI.gameObject.SetActive(true);
        gridContainer.gameObject.SetActive(false); // hide grid
    }

    public void ShowGrid()
    {
        gridContainer.gameObject.SetActive(true);
        RefreshGrid(); // refresh so any purchased upgrades are reflected
    }
}