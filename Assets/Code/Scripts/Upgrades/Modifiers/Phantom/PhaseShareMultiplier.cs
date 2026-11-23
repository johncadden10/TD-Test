using UnityEngine;

// Path 2-3: more lifetime, phasing also phases/regens nearby ghosts
public class PhaseShareModifier : GhostModifier
{
    [SerializeField] private float radius = 2.5f;
    [SerializeField] private float lifetimeMultiplier = 1.5f;
    [SerializeField, Range(0f, 1f)] private float shareFraction = 0.4f;

    protected override void OnAttached()
    {
        ghost.PhaseShareRadius = Mathf.Max(ghost.PhaseShareRadius, radius);
        ghost.LifetimeMultiplier *= lifetimeMultiplier;
        ghost.PhaseShareRegenFraction = Mathf.Max(ghost.PhaseShareRegenFraction, shareFraction);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}