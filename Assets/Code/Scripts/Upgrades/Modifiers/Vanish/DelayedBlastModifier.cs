using UnityEngine;

// Path 2-2: clouds blow up shortly after impact
public class DelayedBlastModifier : VanishModifier
{
    [SerializeField] private float delay = 0.6f;
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float damageMultiplier = 3f; // times the cloud's normal damage

    protected override void Apply()
    {
        vanish.BlastDelay = delay;
        vanish.BlastRadius = Mathf.Max(vanish.BlastRadius, radius);
        vanish.BlastDamageMultiplier = Mathf.Max(vanish.BlastDamageMultiplier, damageMultiplier);
    }
}