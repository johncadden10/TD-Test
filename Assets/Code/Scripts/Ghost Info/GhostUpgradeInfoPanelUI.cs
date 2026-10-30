using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GhostUpgradeInfoPanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI essenceText;

    private string towerId;
    private string pathId;
    private int tierIndex;
    private GhostDetailUI detailUI;

    public void Setup(string tower, string path, int index, UpgradeTier tier, bool purchased, GhostDetailUI detail)
    {
        towerId = tower;
        pathId = path;
        tierIndex = index;
        detailUI = detail;

        upgradeNameText.text = tier.displayName;
        upgradeDescriptionText.text = tier.description;
        upgradeCostText.text = $"Cost: {tier.cost} Essence";
        essenceText.text = $"Essence: {SaveManager.main.CurrentSave.essence}";

        bool canPurchase = UpgradeManager.main.CanPurchaseUpgrade(tower, path, index);
        Debug.Log($"Tower: {tower}, Path: {path}, Tier: {index}, Purchased: {purchased}, CanPurchase: {canPurchase}, Essence: {SaveManager.main.CurrentSave.essence}, Tier cost: {tier.cost}");
        purchaseButton.gameObject.SetActive(!purchased && canPurchase);
    }

    public void OnPurchasePressed()
    {
        bool success = UpgradeManager.main.PurchaseUpgrade(towerId, pathId, tierIndex);

        if (success)
        {
            essenceText.text = $"Essence: {SaveManager.main.CurrentSave.essence}";
            purchaseButton.gameObject.SetActive(false);
            detailUI.RefreshUpgradePaths(); // ← refresh tier colors after purchase
        }
        else
        {
            Debug.Log("Cannot purchase — insufficient Essence or requirements not met");
        }
    }

    public void OnClosePressed()
    {
        gameObject.SetActive(false);
    }
}