using UnityEngine;

// Path 1-2
public class VengefulBoostModifier : ReaperModifier
{
    [SerializeField] private float damageMultiplier = 2f;
    [SerializeField] private float splashRadius = 1.5f;

    protected override void Apply()
    {
        reaper.Vengeful.damageMultiplier *= damageMultiplier;
        reaper.Vengeful.splashRadius = Mathf.Max(reaper.Vengeful.splashRadius, splashRadius);
    }
}