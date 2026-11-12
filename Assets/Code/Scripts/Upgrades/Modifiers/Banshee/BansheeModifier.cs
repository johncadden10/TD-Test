using UnityEngine;
// BansheeModifier.cs
public abstract class BansheeModifier : GhostModifier
{
    protected Banshee banshee;

    protected override void OnAttached()
    {
        banshee = (Banshee)ghost;
        Apply();
    }

    protected abstract void Apply();
}