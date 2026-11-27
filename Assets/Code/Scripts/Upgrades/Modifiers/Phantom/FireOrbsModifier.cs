using UnityEngine;

// Phantom 2-1: fire orbs (burn comes from the status system)
public class FireOrbsModifier : PhantomModifier
{
    protected override void Apply() { phantom.DamageType |= DamageType.Fire; }
}