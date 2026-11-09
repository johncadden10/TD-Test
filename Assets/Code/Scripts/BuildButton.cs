using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuildButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image cooldownOverlay;        // Image Type: Filled, Radial 360
    [SerializeField] private TextMeshProUGUI cooldownText; // optional
    [SerializeField] private Color affordableColor = Color.white;
    [SerializeField] private Color unaffordableColor = new Color(1f, 1f, 1f, 0.5f);

    private LevelLoadout.Entry entry;

    public void Setup(LevelLoadout.Entry e)
    {
        entry = e;
        iconImage.sprite = e.tower.icon;
        costText.text = e.tower.cost.ToString();
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
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (BuildManager.main.IsOnCooldown(entry)) return;

        if (entry.tower.cost > LevelManager.main.ecto)
        {
            Debug.Log("Insufficient Ecto!");
            return;
        }

        BuildManager.main.BeginSelect(entry);
    }
}