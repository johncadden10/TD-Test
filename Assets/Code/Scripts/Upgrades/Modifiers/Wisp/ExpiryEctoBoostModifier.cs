using UnityEngine;

// Path 1-3: bigger expiry payout and more ecto per ghost
public class ExpiryEctoBoostModifier : WispModifier
{
    [SerializeField] private float pulseRateMultiplier = 1.5f;

    protected override void Apply()
    {
        wisp.FireRateBonus *= pulseRateMultiplier;
    }
}