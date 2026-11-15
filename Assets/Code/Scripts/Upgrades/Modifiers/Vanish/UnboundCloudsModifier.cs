using UnityEngine;

public class UnboundCloudsModifier : VanishModifier
{
    [SerializeField] private float lifetimeMultiplier = 1.5f;

    protected override void Apply()
    {
        vanish.LifetimeMultiplier *= lifetimeMultiplier;
        vanish.CloudsIgnoreAbsorbers = true;
        vanish.DamageType &= ~DamageType.Magic;
    }
}