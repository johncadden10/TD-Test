using UnityEngine;

// Path 2-1: the beam hits every star in range
public class MultiBeamModifier : SpiritModifier
{
    protected override void Apply() { spirit.HitsAll = true; }
}
