using UnityEngine;

// Path 1-3: bigger expiry payout and more ecto per ghost
public class ExpiryEctoBoostModifier : WispModifier
{
    [SerializeField] private int ecto = 200;
    [SerializeField] private int bonusPerGhost = 3;

    protected override void Apply()
    {
        wisp.ExpireEcto = Mathf.Max(wisp.ExpireEcto, ecto);
        wisp.BonusEctoPerGhost += bonusPerGhost;
    }
}