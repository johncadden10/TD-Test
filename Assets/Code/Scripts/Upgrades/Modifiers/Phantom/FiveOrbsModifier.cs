using UnityEngine;

// Path 1-3: five orbs in different directions, better tracking
public class FiveOrbsModifier : PhantomModifier
{
    [SerializeField] private float spread = 100f;
    [SerializeField] private float turnRateMultiplier = 2.5f;

    protected override void Apply()
    {
        phantom.ShotCount = Mathf.Max(phantom.ShotCount, 5);
        phantom.SpreadAngle = Mathf.Max(phantom.SpreadAngle, spread);
        phantom.TurnRateMultiplier *= turnRateMultiplier;
    }
}