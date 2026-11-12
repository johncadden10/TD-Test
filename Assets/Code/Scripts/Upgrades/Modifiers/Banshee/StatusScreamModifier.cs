using UnityEngine;

// Path 1-1: bonus damage to stars with any status
public class StatusScreamModifier : GhostModifier
{
    [SerializeField] private float multiplier = 1.5f;
    protected override void OnAttached() { ghost.StatusBonusMultiplier = Mathf.Max(ghost.StatusBonusMultiplier, multiplier); }
}