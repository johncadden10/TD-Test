using UnityEngine;

// Path 2-3: more range and attack speed
public class FrostSurgeModifier : GhostModifier
{
    [SerializeField] private float rangeMultiplier = 1.4f;
    [SerializeField] private float fireRateMultiplier = 1.4f;

    protected override void OnAttached()
    {
        ghost.RangeMultiplier *= rangeMultiplier;
        ghost.FireRateBonus *= fireRateMultiplier;
    }
}