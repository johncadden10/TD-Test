using UnityEngine;

// Path 1-1: ecto when the Wisp expires
public class ExpiryEctoModifier : WispModifier
{
    [SerializeField] private int ecto = 100;
    protected override void Apply() { wisp.ExpireEcto = Mathf.Max(wisp.ExpireEcto, ecto); }
}