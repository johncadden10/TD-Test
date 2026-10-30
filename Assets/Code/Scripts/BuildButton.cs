using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BuildButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI costText;

    private LevelLoadout.Entry entry;

    public void Setup(LevelLoadout.Entry e)
    {
        entry = e;
        iconImage.sprite = e.tower.icon;
        costText.text = e.tower.cost.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (entry.tower.cost > LevelManager.main.ecto)
        {
            Debug.Log("Insufficient Ecto!");
            return;
        }
        BuildManager.main.BeginSelect(entry);
    }
}