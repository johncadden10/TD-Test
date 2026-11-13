using UnityEngine;

// Path 1-1: sharper shards pierce and fly faster
public class SharperShardsModifier : PoltergeistModifier
{
    [SerializeField] private int extraPierce = 1;
    [SerializeField] private float speedMultiplier = 1.3f;

    protected override void Apply()
    {
        poltergeist.PierceBonus += extraPierce;
        poltergeist.ShardSpeedMultiplier *= speedMultiplier;
    }
}