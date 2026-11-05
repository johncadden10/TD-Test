using System;

[Flags]
public enum DamageType
{
    Neutral = 0,
    Physical = 1,
    Magic = 2,
    Fire = 4,
    Cold = 8
}

public static class DamageTypeExtensions
{
    // true if the hit carries any of the given flags
    public static bool Has(this DamageType type, DamageType flag)
    {
        return (type & flag) != 0;
    }
}