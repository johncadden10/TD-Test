using UnityEngine;
using UnityEngine.UI;

public class TowerHealthBar : MonoBehaviour
{
    [Header("Overheal")]
    [SerializeField] private Image overhealFill;   // Filled, Horizontal, drawn above the normal fill
    [SerializeField] private Image normalFill;     // optional: the slider's Fill image, tinted while overhealed
    [SerializeField] private Color overhealTint = new Color(1f, 0.85f, 0.2f);

    [SerializeField] private Slider slider;

    private Color baseColor = Color.white;

    public void SetProgress(float percent)
    {
        slider.value = percent;
    }

    public void SetLifetime(float remaining, float max, float overhealCap)
    {
        slider.value = Mathf.Clamp01(remaining / max);

        // a full overheal bar equals the overheal cap in seconds, not a fraction of this ghost's max
        float over = overhealCap > 0f ? Mathf.Clamp01((remaining - max) / overhealCap) : 0f;
        bool overhealed = remaining > max;

        if (overhealFill != null)
        {
            overhealFill.enabled = overhealed;
            overhealFill.fillAmount = over;
        }
        if (normalFill != null)
            normalFill.color = overhealed ? overhealTint : baseColor;
    }

    private void Awake()
    {
        if (normalFill != null) baseColor = normalFill.color;
        if (overhealFill != null) overhealFill.enabled = false;
    }

    private void Update()
    {
        // counteract parent flip on x axis
        float parentScaleX = transform.parent.lossyScale.x;
        transform.localScale = new Vector3(
            parentScaleX < 0 ? -1f : 1f,
            1f,
            1f
        );
    }
}