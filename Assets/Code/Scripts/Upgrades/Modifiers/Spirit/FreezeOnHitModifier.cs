using UnityEngine;

// Path 1-1: hit stars are frozen for the duration of the chill
public class FreezeOnHitModifier : SpiritModifier
{
    protected override void Apply() { spirit.FreezeOnHit = true; }
}
