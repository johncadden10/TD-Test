using UnityEngine;

// Path 2-1: phase out when attacked by a star
public class PhaseOnAttackModifier : GhostModifier
{
    [SerializeField] private float duration = 3f;
    [SerializeField] private float cooldown = 5f;

    protected override void OnAttached()
    {
        ghost.PhaseOnAttack = true;
        ghost.PhaseDuration = duration;
        ghost.PhaseCooldown = cooldown;
    }
}