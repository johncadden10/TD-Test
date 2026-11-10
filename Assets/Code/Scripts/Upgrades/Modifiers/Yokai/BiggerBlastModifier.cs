using UnityEngine;

// Path 1-3: bigger, harder blast
public class BiggerBlastModifier : YokaiModifier
{
    [SerializeField] private float damageMultiplier = 1.5f;
    [SerializeField] private float rangeMultiplier = 1.3f;

    protected override void Apply()
    {
        yokai.ExplosionDamageMultiplier *= damageMultiplier;
        yokai.RangeMultiplier *= rangeMultiplier;
    }
}