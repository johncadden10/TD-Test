using UnityEngine;

// Path 1-1: ecto when the Wisp expires
public class ExpiryEctoModifier : WispModifier
{
    [SerializeField] private int flatEcto = 10;
    [SerializeField] private float lifetimeMultiplier = 1.8f;
    
    protected override void Apply() 
    { 
        wisp.FlatEctoPerBurst = Mathf.Max(wisp.FlatEctoPerBurst, flatEcto); 
        wisp.LifetimeMultiplier *= lifetimeMultiplier;
    }
}