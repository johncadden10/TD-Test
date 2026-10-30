using UnityEngine;
using UnityEngine.UI;

public class TowerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetProgress(float percent)
    {
        slider.value = percent;
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