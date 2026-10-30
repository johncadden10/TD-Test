using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button button;

    private LevelLoadout.Entry entry;

    public void Setup(LevelLoadout.Entry e)
    {
        entry = e;
        iconImage.sprite = e.tower.icon;
        button.onClick.AddListener(() => LevelUpgradeManager.main.TryBuyNext(entry));
        LevelUpgradeManager.main.onUpgradesChanged.AddListener(Refresh);
        Refresh();
    }

    private void Update()
    {
        Refresh(); // keeps affordability current as Ecto changes
    }

    private void Refresh()
    {
        UpgradeTier next = LevelUpgradeManager.main.GetNextTier(entry);

        if (next == null)
        {
            nameText.text = "Maxed";
            costText.text = "";
            button.interactable = false;
        }
        else if (!LevelUpgradeManager.main.IsUnlockedInHub(next))
        {
            nameText.text = next.displayName;
            costText.text = "Locked";
            button.interactable = false;
        }
        else
        {
            nameText.text = next.displayName;
            costText.text = next.ectoCost.ToString();
            button.interactable = LevelManager.main.ecto >= next.ectoCost;
        }
    }
}