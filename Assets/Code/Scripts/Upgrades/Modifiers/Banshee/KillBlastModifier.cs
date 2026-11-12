using UnityEngine;

// Path 2-3: stars that die in the radius explode
public class KillBlastModifier : BansheeModifier
{
    [SerializeField] private float radius = 2f;
    [SerializeField] private float damageMultiplier = 5f; // times the banshee's attackDamage

    protected override void Apply()
    {
        banshee.KillBlastRadius = Mathf.Max(banshee.KillBlastRadius, radius);
        banshee.KillBlastMultiplier = Mathf.Max(banshee.KillBlastMultiplier, damageMultiplier);
    }
}