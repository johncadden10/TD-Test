using UnityEngine;
using UnityEngine.UI;

public class UpgradeTierButtonUI : MonoBehaviour
{
    [SerializeField] private Image tierImage;
    [SerializeField] private Color purchasedColor = Color.cyan;
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color unavailableColor = Color.grey;
    [SerializeField] private Color inactivePathColor = new Color(1f, 1f, 1f, 0.3f);

    private UpgradeTier tierData;
    private UpgradePathUI pathUI;

    public void Setup(UpgradeTier tier, bool purchased, bool available, bool isSelectedPath, UpgradePathUI path)
    {
        tierData = tier;
        pathUI = path;

        if (!isSelectedPath)
        {
            tierImage.color = inactivePathColor;
            GetComponent<Button>().interactable = false;
            return;
        }

        if (purchased)
            tierImage.color = purchasedColor;
        else if (available)
            tierImage.color = availableColor;
        else
            tierImage.color = unavailableColor;

        GetComponent<Button>().interactable = available || purchased;
    }

    public void OnTierPressed()
    {
        pathUI.ShowUpgradeInfo(tierData);
    }
}