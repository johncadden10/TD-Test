using UnityEngine;

public abstract class ReaperModifier : GhostModifier
{
    protected Reaper reaper;

    protected override void OnAttached()
    {
        reaper = (Reaper)ghost;
        Apply();
    }

    protected abstract void Apply();
}
