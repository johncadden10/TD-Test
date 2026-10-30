using UnityEngine;

// Path 1-1
public class VengefulSoulsModifier : ReaperModifier
{
    protected override void Apply() { reaper.Vengeful.enabled = true; }
}