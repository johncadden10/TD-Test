// Path 2-1: ghosts in the radius stop draining lifetime
public class StillTimeModifier : WispModifier
{
    protected override void Apply() { wisp.PauseDrain = true; }
}