// VanishModifier.cs
public abstract class VanishModifier : GhostModifier
{
    protected Vanish vanish;

    protected override void OnAttached()
    {
        vanish = (Vanish)ghost;
        Apply();
    }

    protected abstract void Apply();
}