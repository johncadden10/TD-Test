using UnityEngine;

// Path 1-1: two orbs at once
public class TwoOrbsModifier : PhantomModifier
{
    [SerializeField] private float spread = 14f;

    protected override void Apply()
    {
        phantom.ShotCount = Mathf.Max(phantom.ShotCount, 2);
        phantom.SpreadAngle = Mathf.Max(phantom.SpreadAngle, spread);
    }
}