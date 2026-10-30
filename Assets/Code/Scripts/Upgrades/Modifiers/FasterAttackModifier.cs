using UnityEngine;

public class FasterAttackModifier : GhostModifier
{
    [SerializeField] private float multiplier = 1.25f;

    protected override void OnAttached()
    {
        ghost.AttackSpeedMultiplier *= multiplier;
    }
}