// WispModifier.cs
public abstract class WispModifier : GhostModifier
{
    protected Wisp wisp;

    protected override void OnAttached()
    {
        wisp = (Wisp)ghost;
        Apply();
    }

    protected abstract void Apply();
}