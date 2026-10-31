using UnityEngine;

public interface IDamageModifier
{
    int Order { get; }                  // lower runs first
    void Modify(ref DamageInfo info);   // set amount to 0 to negate
}
