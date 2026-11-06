using UnityEngine;

public abstract class PhantomModifier : GhostModifier
{
    protected Phantom phantom;

    protected override void OnAttached()
    {
        phantom = (Phantom)ghost;
        Apply();
    }

    protected abstract void Apply();
}