using UnityEngine;

// Path 1-2: much longer life; clouds ignore belted
public class DurableCloudsModifier : VanishModifier
{
    [SerializeField] private float lifetimeMultiplier = 2f;

    protected override void Apply()
    {
        vanish.LifetimeMultiplier *= lifetimeMultiplier;
        vanish.CloudsIgnoreBelted = true;
    }
}