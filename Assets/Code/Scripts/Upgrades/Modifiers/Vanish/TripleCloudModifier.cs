using UnityEngine;

// Path 2-3: three clouds at once
public class TripleCloudModifier : VanishModifier
{
    [SerializeField] private float spread = 24f;

    protected override void Apply()
    {
        vanish.ShotCount = Mathf.Max(vanish.ShotCount, 3);
        vanish.SpreadAngle = Mathf.Max(vanish.SpreadAngle, spread);
    }
}