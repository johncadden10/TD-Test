using UnityEngine;

public class DurableCloudsModifier : VanishModifier
{
    [SerializeField] private float rangeMultiplier = 1.3f;

    protected override void Apply()
    {
        vanish.RangeMultiplier *= rangeMultiplier;
        vanish.CloudsIgnoreBelted = true;
    }
}