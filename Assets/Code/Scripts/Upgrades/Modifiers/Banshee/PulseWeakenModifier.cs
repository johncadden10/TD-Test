using UnityEngine;

// Path 1-3: the pulse also weakens stars
public class PulseWeakenModifier : BansheeModifier
{
    [SerializeField] private float weakenMultiplier = 0.5f;

    protected override void Apply()
    {
        banshee.PulseWeakens = true;
        banshee.WeakenMultiplier = Mathf.Min(banshee.WeakenMultiplier, weakenMultiplier);
    }
}