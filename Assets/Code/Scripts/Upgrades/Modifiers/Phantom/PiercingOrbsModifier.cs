using UnityEngine;

// Path 1-2: faster orbs that pierce one star with advanced tracking
public class PiercingOrbsModifier : PhantomModifier
{
    [SerializeField] private float speedMultiplier = 1.4f;
    [SerializeField] private int extraPierce = 1;
    [SerializeField] private float turnRateMultiplier = 3f;

    protected override void Apply()
    {
        phantom.OrbSpeedMultiplier *= speedMultiplier;
        phantom.PierceBonus += extraPierce;
        phantom.OrbsReseek = true;
        phantom.TurnRateMultiplier *= turnRateMultiplier;
    }
}