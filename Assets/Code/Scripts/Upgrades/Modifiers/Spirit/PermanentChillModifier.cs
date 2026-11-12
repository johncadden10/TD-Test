using UnityEngine;

// Path 1-2: hit stars stay chilled after thawing
public class PermanentChillModifier : SpiritModifier
{
    protected override void Apply() { spirit.PermanentChill = true; }
}
