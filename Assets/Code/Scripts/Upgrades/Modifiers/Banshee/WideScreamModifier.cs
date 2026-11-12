using UnityEngine;

// Path 2-2: drastically larger scream radius
public class WideScreamModifier : BansheeModifier
{
    [SerializeField] private float rangeMultiplier = 2f;
    protected override void Apply() { banshee.RangeMultiplier *= rangeMultiplier; }
}