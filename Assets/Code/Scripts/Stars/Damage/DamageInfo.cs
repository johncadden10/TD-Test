using UnityEngine;

public struct DamageInfo
{
    public float amount;
    public DamageType type;
    public GameObject source;
    public bool fromStatus;
    public bool ignoreHidden; // may hit an untargetable (hidden) star

    public DamageInfo(float amount, DamageType type = DamageType.Neutral, GameObject source = null,
                      bool fromStatus = false, bool ignoreHidden = false)
    {
        this.amount = amount;
        this.type = type;
        this.source = source;
        this.fromStatus = fromStatus;
        this.ignoreHidden = ignoreHidden;
    }
}