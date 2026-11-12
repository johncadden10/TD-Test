using UnityEngine;

public class BeltedModifier : MonoBehaviour, IDamageModifier
{
    public float physicalMultiplier = 0.5f;
    public float maxHit = 1f;

    public int Order => 30;

    private bool stripped;

    public void Modify(ref DamageInfo info)
    {
        if (stripped || info.ignoreBelted) return; // skips both the physical resist and the clamp to 1
        
        if (info.type.Has(DamageType.Physical))
            info.amount *= physicalMultiplier;

        if (info.amount > maxHit)
            info.amount = maxHit;
    }
    
    public void Strip()
    {
        stripped = true;
        Transform overlay = transform.Find("BeltedOverlay");
        if (overlay != null) Destroy(overlay.gameObject);
    }
}