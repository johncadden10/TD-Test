using UnityEngine;

public class FasterHarvestModifier : ReaperModifier
{
    [SerializeField] private float speedMultiplier = 1.75f;
    [SerializeField] private float valueMultiplier = 0.7f;

    protected override void Apply()
    {
        reaper.SpeedMultiplier *= speedMultiplier;
        reaper.ValueMultiplier *= valueMultiplier;
    }
}