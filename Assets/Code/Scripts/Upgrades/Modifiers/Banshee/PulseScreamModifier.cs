using UnityEngine;
// Path 1-2: pulse scream that slows and strips belted
public class PulseScreamModifier : BansheeModifier
{
    [SerializeField] private int pierceBonus = 1;

    protected override void Apply()
    {
        banshee.PulseEnabled = true;
        banshee.PulsePierceBonus = Mathf.Max(banshee.PulsePierceBonus, pierceBonus);
    }
}