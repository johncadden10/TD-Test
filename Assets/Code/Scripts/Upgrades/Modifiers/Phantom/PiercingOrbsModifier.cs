using UnityEngine;

// Path 1-2: faster orbs that pierce one star
public class PiercingOrbsModifier : PhantomModifier
{
    [SerializeField] private float speedMultiplier = 1.4f;
    [SerializeField] private int extraPierce = 1;

    protected override void Apply()
    {
        phantom.OrbSpeedMultiplier *= speedMultiplier;
        phantom.PierceBonus += extraPierce;
    }
}