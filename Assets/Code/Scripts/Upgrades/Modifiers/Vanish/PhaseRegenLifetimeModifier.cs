using UnityEngine;

// Path 2-2: regen on phasing
public class PhaseRegenLifetimeModifier : GhostModifier
{
    [SerializeField] private float lifetimeRegen = 8f;

    protected override void OnAttached()
    {
        ghost.PhaseLifetimeRegen += lifetimeRegen;
    }
}