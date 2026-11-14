using UnityEngine;

// Path 2-3: ghosts in the radius regain lifetime
public class RenewalModifier : WispModifier
{
    [SerializeField] private float secondsPerSecond = 2f;
    protected override void Apply() { wisp.ReplenishPerSecond = Mathf.Max(wisp.ReplenishPerSecond, secondsPerSecond); }
}