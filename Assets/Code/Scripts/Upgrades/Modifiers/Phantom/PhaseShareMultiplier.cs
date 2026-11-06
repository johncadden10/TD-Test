using UnityEngine;

// Path 2-2: phasing also phases nearby ghosts
public class PhaseShareModifier : GhostModifier
{
    [SerializeField] private float radius = 2.5f;

    protected override void OnAttached()
    {
        ghost.PhaseShareRadius = Mathf.Max(ghost.PhaseShareRadius, radius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}