using UnityEngine;
using UnityEngine.UI;

public class ConstellationLine : MonoBehaviour
{
    [SerializeField] private Image lineImage;
    [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.4f);

    public void Setup(Vector2 from, Vector2 to)
    {
        lineImage.color = lineColor;

        Vector2 direction = to - from;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchoredPosition = from + direction * 0.5f;
        rt.sizeDelta = new Vector2(distance, rt.sizeDelta.y); // line thickness set in prefab
        rt.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}