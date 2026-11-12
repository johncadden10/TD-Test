// SpiritModifier.cs
public abstract class SpiritModifier : GhostModifier
{
    protected Spirit spirit;

    protected override void OnAttached()
    {
        spirit = (Spirit)ghost;
        Apply();
    }

    protected abstract void Apply();
}