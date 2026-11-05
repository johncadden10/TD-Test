using UnityEngine;

public struct DamageInfo
{
    public float amount;
    public DamageType type;
    public GameObject source;

    public DamageInfo(float amount, DamageType type = DamageType.Neutral, GameObject source = null)
    {
        this.amount = amount;
        this.type = type;
        this.source = source;
    }
}