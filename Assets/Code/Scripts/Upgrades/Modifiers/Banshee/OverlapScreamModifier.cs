using UnityEngine;

// Path 2-1: overlapping screams hit much harder
public class OverlapScreamModifier : BansheeModifier
{
    [SerializeField] private float multiplier = 2.5f;
    protected override void Apply() { banshee.OverlapMultiplier = Mathf.Max(banshee.OverlapMultiplier, multiplier); }
}