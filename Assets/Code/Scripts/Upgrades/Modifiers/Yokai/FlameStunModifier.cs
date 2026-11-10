// Path 1-2: flame damage stuns until the flames end
public class FlameStunModifier : YokaiModifier
{
    protected override void Apply() { yokai.FlamesStun = true; }
}