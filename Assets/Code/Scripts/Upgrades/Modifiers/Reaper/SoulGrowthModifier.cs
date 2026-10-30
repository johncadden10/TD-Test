using UnityEngine;

// Path 2-3
public class SoulGrowthModifier : ReaperModifier
{
    [SerializeField] private float growthPerSecond = 0.5f;
    [SerializeField] private int maxBonus = 30;

    protected override void Apply()
    {
        reaper.ValueGrowthPerSecond = growthPerSecond;
        reaper.MaxValueBonus = maxBonus;
    }
}