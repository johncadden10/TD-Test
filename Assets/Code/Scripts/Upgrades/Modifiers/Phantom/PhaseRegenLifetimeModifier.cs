using UnityEngine;

// Path 2-3: more lifetime, regen on phasing, and a weaker share for nearby ghosts
public class PhaseRegenLifetimeModifier : GhostModifier
{
    [SerializeField] private float lifetimeMultiplier = 1.5f;
    [SerializeField] private float lifetimeRegen = 8f;
    [SerializeField, Range(0f, 1f)] private float shareFraction = 0.4f;

    protected override void OnAttached()
    {
        ghost.LifetimeMultiplier *= lifetimeMultiplier;
        ghost.PhaseLifetimeRegen += lifetimeRegen;
        ghost.PhaseShareRegenFraction = Mathf.Max(ghost.PhaseShareRegenFraction, shareFraction);
    }
}