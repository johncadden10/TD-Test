using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI keyText; // optional

    private KeybindAction action;
    private LevelLoadout.Entry entry;

    public void Setup(LevelLoadout.Entry e, int slotIndex)
    {
        entry = e;
        action = KeybindAction.UpgradeSlot1 + Mathf.Clamp(slotIndex, 0, 3);
        iconImage.sprite = e.tower.icon;
        button.onClick.AddListener(() => LevelUpgradeManager.main.TryBuyNext(entry));
        LevelUpgradeManager.main.onUpgradesChanged.AddListener(Refresh);
        KeybindManager.OnBindingsChanged += RefreshKey;
        RefreshKey();
        Refresh();
    }

    private void Update()
    {
        Refresh(); // keeps affordability current as Ecto changes
        // same as a click: only works while the button is interactable
        if (button.interactable && KeybindManager.Pressed(action))
            LevelUpgradeManager.main.TryBuyNext(entry);
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

    private void OnDestroy() { KeybindManager.OnBindingsChanged -= RefreshKey; }

    private void RefreshKey()
    {
        if (keyText != null) keyText.text = KeybindManager.KeyLabel(action);
    }
}