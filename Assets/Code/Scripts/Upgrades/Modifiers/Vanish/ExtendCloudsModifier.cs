using UnityEngine;

public class ExtendedCloudsModifier : VanishModifier
{
    [SerializeField] private int extraPierce = 1;
    [SerializeField] private float lifetimeMultiplier = 1.5f;

    protected override void Apply()
    {
        vanish.PierceBonus += extraPierce;
        vanish.LifetimeMultiplier *= lifetimeMultiplier;
    }
}