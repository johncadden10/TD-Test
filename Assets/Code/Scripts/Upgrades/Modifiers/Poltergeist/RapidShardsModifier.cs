using UnityEngine;

// Path 1-3: much faster attack speed (also speeds up the animator)
public class RapidShardsModifier : GhostModifier
{
    [SerializeField] private float rateMultiplier = 2f;
    protected override void OnAttached() { ghost.FireRateBonus *= rateMultiplier; }
}