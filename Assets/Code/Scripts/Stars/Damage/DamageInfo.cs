using UnityEngine;

public struct DamageInfo
{
    public int amount;
    public DamageType type;
    public GameObject source;

    public DamageInfo(int amount, DamageType type = DamageType.Physical, GameObject source = null)
    {
        this.amount = amount;
        this.type = type;
        this.source = source;
    }
}