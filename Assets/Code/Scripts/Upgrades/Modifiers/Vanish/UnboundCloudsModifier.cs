using UnityEngine;
// Path 1-3: clouds stop being projectiles and lose the magic type
public class UnboundCloudsModifier : VanishModifier
{
    [SerializeField] private float turnRateMultiplier = 2f;

    protected override void Apply()
    {
        vanish.CloudsIgnoreAbsorbers = true;
        vanish.DamageType &= ~DamageType.Magic;
        vanish.CloudsReseek = true;
        vanish.TurnRateMultiplier *= turnRateMultiplier;
    }
}