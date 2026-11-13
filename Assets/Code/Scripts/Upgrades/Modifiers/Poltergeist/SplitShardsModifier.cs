using UnityEngine;

// Path 1-2: shards split into fragments when their travel time ends
public class SplitShardsModifier : PoltergeistModifier
{
    [SerializeField] private int fragments = 3;
    [SerializeField] private float fragmentDamageMultiplier = 0.5f;
    [SerializeField] private float fragmentLifetime = 0.8f;

    protected override void Apply()
    {
        poltergeist.SplitCount = Mathf.Max(poltergeist.SplitCount, fragments);
        poltergeist.SplitDamageMultiplier = fragmentDamageMultiplier;
        poltergeist.SplitLifetime = fragmentLifetime;
    }
}