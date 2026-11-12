using UnityEngine;

// Path 1-3: hits make chilled stars vulnerable and weakened
public class FrostDebuffModifier : SpiritModifier
{
    protected override void Apply() { spirit.DebuffsChilled = true; }
}
