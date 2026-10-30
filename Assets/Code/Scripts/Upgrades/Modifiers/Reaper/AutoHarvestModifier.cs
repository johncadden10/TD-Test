using UnityEngine;

// Path 2-1
public class AutoHarvestModifier : ReaperModifier
{
    protected override void Apply() { reaper.AutoCollect = true; }
}
