using UnityEngine;

// Path 2-1: greatly increased attack speed
public class RapidFireModifier : VanishModifier
{
    [SerializeField] private float rateMultiplier = 2f;
    protected override void Apply() { vanish.FireRateBonus *= rateMultiplier; }
}