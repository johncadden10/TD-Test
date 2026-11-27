using UnityEngine;

// Phantom 2-2: orbs grow in damage and size as they travel; more range
public class GrowingOrbsModifier : PhantomModifier
{
    [SerializeField] private float damageGrowthPerSecond = 0.75f;
    [SerializeField] private float sizeGrowthPerSecond = 0.5f;
    [SerializeField] private float growthCap = 3f;
    [SerializeField] private float rangeMultiplier = 1.4f;

    protected override void Apply()
    {
        phantom.OrbDamageGrowth = Mathf.Max(phantom.OrbDamageGrowth, damageGrowthPerSecond);
        phantom.OrbSizeGrowth = Mathf.Max(phantom.OrbSizeGrowth, sizeGrowthPerSecond);
        phantom.OrbGrowthCap = Mathf.Max(phantom.OrbGrowthCap, growthCap);
        phantom.RangeMultiplier *= rangeMultiplier;       // targeting radius
        phantom.OrbLifetimeMultiplier *= rangeMultiplier; // so orbs actually reach it
    }
}