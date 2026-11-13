using UnityEngine;

// Path 2-3: boulders stun in a radius and hit fused stars harder
public class BoulderImpactModifier : PoltergeistModifier
{
    [SerializeField] private float stunSeconds = 1.5f;
    [SerializeField] private float stunRadius = 1.5f;
    [SerializeField] private float fusedBonus = 2f;

    protected override void Apply()
    {
        poltergeist.BoulderStun = Mathf.Max(poltergeist.BoulderStun, stunSeconds);
        poltergeist.BoulderStunRadius = Mathf.Max(poltergeist.BoulderStunRadius, stunRadius);
        poltergeist.BoulderFusedBonus = Mathf.Max(poltergeist.BoulderFusedBonus, fusedBonus);
    }
}