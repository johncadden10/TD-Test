using UnityEngine;

// Path 2-2: bigger radius and longer life
public class WideWispModifier : WispModifier
{
    [SerializeField] private float rangeMultiplier = 1.5f;

    protected override void Apply()
    {
        wisp.RangeMultiplier *= rangeMultiplier;
    }
}