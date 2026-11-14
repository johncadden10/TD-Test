using UnityEngine;

// Path 1-2: ghosts expiring in range refund part of their cost; Wisps stack up to the cap
public class RefundModifier : WispModifier
{
    [SerializeField, Range(0f, 1f)] private float fraction = 0.25f;
    [SerializeField, Range(0f, 1f)] private float cap = 0.5f;

    protected override void Apply()
    {
        wisp.RefundFraction = Mathf.Max(wisp.RefundFraction, fraction);
        wisp.RefundCap = Mathf.Max(wisp.RefundCap, cap);
    }
}