using UnityEngine;

// PoltergeistModifier.cs
public abstract class PoltergeistModifier : GhostModifier
{
    protected Poltergeist poltergeist;

    protected override void OnAttached()
    {
        poltergeist = (Poltergeist)ghost;
        Apply();
    }

    protected abstract void Apply();
}
