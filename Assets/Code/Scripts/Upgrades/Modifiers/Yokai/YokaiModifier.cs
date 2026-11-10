using UnityEngine;

// YokaiModifier.cs
public abstract class YokaiModifier : GhostModifier
{
    protected Yokai yokai;

    protected override void OnAttached()
    {
        yokai = (Yokai)ghost;
        Apply();
    }

    protected abstract void Apply();
}
