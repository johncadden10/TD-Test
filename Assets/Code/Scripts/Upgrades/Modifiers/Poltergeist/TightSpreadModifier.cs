using UnityEngine;

// Path 2-1: tighter spread
public class TightSpreadModifier : PoltergeistModifier
{
    [SerializeField] private float spreadMultiplier = 0.4f;
    protected override void Apply() { poltergeist.SpreadMultiplier *= spreadMultiplier; }
}