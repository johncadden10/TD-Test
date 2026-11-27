using UnityEngine;

// Phantom 2-3: orbs leave flames where they hit, and ignore black holes
public class EmberOrbsModifier : PhantomModifier
{
    [SerializeField] private float flameDuration = 4f;

    protected override void Apply()
    {
        phantom.FlameDuration = Mathf.Max(phantom.FlameDuration, flameDuration);
        phantom.OrbsIgnoreAbsorbers = true;
    }
}