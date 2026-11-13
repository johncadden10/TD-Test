// Path 2-2: the attack becomes a boulder that bursts into debris
public class BoulderModifier : PoltergeistModifier
{
    protected override void Apply() { poltergeist.BoulderMode = true; }
}