using UnityEngine;

// Path 2-2: taking damage freezes everything on screen
public class ColdSnapModifier : SpiritModifier
{
    protected override void Apply() { spirit.ColdSnap = true; }
}
