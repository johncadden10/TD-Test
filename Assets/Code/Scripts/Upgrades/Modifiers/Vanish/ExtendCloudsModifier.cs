using UnityEngine;

// Path 1-1: range, pierce, cloud speed
public class ExtendedCloudsModifier : VanishModifier
{
    [SerializeField] private float rangeMultiplier = 1.3f;
    [SerializeField] private int extraPierce = 1;
    [SerializeField] private float speedMultiplier = 1.3f;

    protected override void Apply()
    {
        vanish.RangeMultiplier *= rangeMultiplier;
        vanish.PierceBonus += extraPierce;
        vanish.CloudSpeedMultiplier *= speedMultiplier;
    }
}