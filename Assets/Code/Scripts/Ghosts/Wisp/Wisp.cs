using UnityEngine;

public class Wisp : Ghost
{
    [Header("Wisp References")]
    [SerializeField] private GameObject pulseEffectPrefab; // optional, scaled to the radius

    [Header("Wisp Attributes")]
    [SerializeField] private int ectoPerGhost = 5;

    // changed by upgrade modifiers
    public float EctoMultiplier { get; set; } = 1f;
    public int BonusEctoPerGhost { get; set; } = 0;

    private float progress;
    private bool isPulsing;

    // a Wisp never targets or attacks
    protected override void Shoot() { }

    protected override void Update()
    {
        if (IsStunned || isPulsing) return;

        // attackSpeed is the seconds between ticks; haste and fire rate upgrades speed it up
        progress += HasteDelta;

        if (progress >= attackSpeed)
        {
            progress = 0f;

            Pulse();
        }
    }

    private void Pulse()
    {
        int count = GhostsInRadius();
        if (count <= 0) return;

        int perGhost = Mathf.RoundToInt((ectoPerGhost + BonusEctoPerGhost) * EctoMultiplier);
        LevelManager.main.IncreaseEcto(perGhost * count);

        if (pulseEffectPrefab != null)
        {
            GameObject fx = Instantiate(pulseEffectPrefab, transform.position, Quaternion.identity);
            fx.transform.localScale = Vector3.one * TargetingRange * 2f; // assumes a 1-unit sprite
        }
    }

    // other ghosts within the radius; the Wisp doesn't count itself
    private int GhostsInRadius()
    {
        int count = 0;
        foreach (Ghost g in Ghost.Active)
        {
            if (g == this || !g.IsAlive) continue;
            if (Vector2.Distance(g.transform.position, transform.position) <= TargetingRange)
                count++;
        }
        return count;
    }

    // other ghosts in range, for future upgrades (buffs, shields and so on)
    public System.Collections.Generic.List<Ghost> GhostsNearby()
    {
        var result = new System.Collections.Generic.List<Ghost>();
        foreach (Ghost g in Ghost.Active)
        {
            if (g == this || !g.IsAlive) continue;
            if (Vector2.Distance(g.transform.position, transform.position) <= TargetingRange)
                result.Add(g);
        }
        return result;
    }
}