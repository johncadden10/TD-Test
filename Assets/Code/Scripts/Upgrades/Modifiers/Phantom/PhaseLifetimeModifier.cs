using UnityEngine;

// Path 2-3: regain lifetime when phasing out, and live longer overall
public class PhaseLifetimeModifier : GhostModifier
{
    [SerializeField] private float lifetimeRegen = 6f;
    [SerializeField] private float lifetimeMultiplier = 1.5f;

    protected override void OnAttached()
    {
        ghost.PhaseLifetimeRegen += lifetimeRegen;
        ghost.LifetimeMultiplier *= lifetimeMultiplier;
    }
}