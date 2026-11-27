using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuildButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI keyText;       // optional hotkey label
    [SerializeField] private Image cooldownOverlay;
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Color affordableColor = Color.white;
    [SerializeField] private Color unaffordableColor = new Color(1f, 1f, 1f, 0.5f);

    private LevelLoadout.Entry entry;
    private KeybindAction action;

    public void Setup(LevelLoadout.Entry e, int slotIndex)
    {
        entry = e;
        action = KeybindAction.BuildSlot1 + Mathf.Clamp(slotIndex, 0, 3);
        iconImage.sprite = e.tower.icon;
        costText.text = e.tower.cost.ToString();
        RefreshKeyLabel();
    }

    private void OnEnable()  { KeybindManager.OnBindingsChanged += RefreshKeyLabel; }
    private void OnDisable() { KeybindManager.OnBindingsChanged -= RefreshKeyLabel; }

    private void RefreshKeyLabel()
    {
        if (keyText != null && entry != null) keyText.text = KeybindManager.KeyLabel(action);
    }

    private void Update()
    {
        if (entry == null) return;

        float remaining = BuildManager.main.CooldownRemaining(entry);
        bool cooling = remaining > 0f;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.enabled = cooling;
            cooldownOverlay.fillAmount = BuildManager.main.CooldownFraction(entry);
        }
        if (cooldownText != null)
            cooldownText.text = cooling ? Mathf.CeilToInt(remaining).ToString() : "";

        iconImage.color = LevelManager.main.ecto >= entry.tower.cost ? affordableColor : unaffordableColor;

        if (KeybindManager.Pressed(action)) Activate(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameSpeed.IsPaused) return;
        Activate(false);
    }

    // identical checks for a click and a key press
    private void Activate(bool fromKeyboard)
    {
        if (BuildManager.main.IsOnCooldown(entry)) return;

        if (entry.tower.cost > LevelManager.main.ecto)
        {
            Debug.Log("Insufficient Ecto!");
            return;
        }

        BuildManager.main.BeginSelect(entry, fromKeyboard);
    }
}