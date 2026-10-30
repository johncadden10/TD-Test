using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GhostUpgradeTierUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image tierImage;
    [SerializeField] private TextMeshProUGUI tierNameText;
    [SerializeField] private Color purchasedColor = Color.cyan;
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color unavailableColor = Color.grey;

    private string towerId;
    private string pathId;
    private int tierIndex;
    private GhostDetailUI detailUI;
    private UpgradeTier tierData;
    private GhostUpgradeInfoPanelUI infoPanel;

    public void Setup(string tower, string path, int index, UpgradeTier tier, bool purchased, bool available, GhostUpgradeInfoPanelUI panel, GhostDetailUI detail)
    {
        towerId = tower;
        pathId = path;
        tierIndex = index;
        tierData = tier;
        infoPanel = panel;
        detailUI = detail;

        tierNameText.text = tier.displayName;

        if (purchased)
            tierImage.color = purchasedColor;
        else if (available)
            tierImage.color = availableColor;
        else
            tierImage.color = unavailableColor;

        GetComponent<Button>().interactable = true; // always pressable to view info
    }

    public void OnTierPressed()
    {
        bool purchased = SaveManager.main.CurrentSave.purchasedUpgrades.Contains(tierData.upgradeId);
        infoPanel.Setup(towerId, pathId, tierIndex, tierData, purchased, detailUI);
        infoPanel.gameObject.SetActive(true);
    }
}