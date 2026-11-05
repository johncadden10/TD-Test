using UnityEngine;

public class FusionModifier : MonoBehaviour, IDamageModifier
{
    public float threshold = 5f;
    public float magicMultiplier = 0.5f;

    public int Order => 20;

    public void Modify(ref DamageInfo info)
    {
        if (info.type.Has(DamageType.Magic))
            info.amount *= magicMultiplier;

        if (info.amount < threshold)
            info.amount = 0f; // below the threshold, the hit does nothing
    }
}