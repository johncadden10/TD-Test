using System.Collections.Generic;
using UnityEngine;

public class Wisp : Ghost
{
    private static readonly List<Wisp> all = new List<Wisp>();

    [Header("Wisp References")]
    [SerializeField] private GameObject pulseEffectPrefab; // optional, scaled to the radius

    [Header("Wisp Attributes")]
    [SerializeField] private int ectoPerGhost = 5;
    [SerializeField] private bool stallYokai = false; // see the notes: Yokai need to expire to explode

    // changed by upgrade modifiers
    public float EctoMultiplier { get; set; } = 1f;
    public int BonusEctoPerGhost { get; set; }
    public int ExpireEcto { get; set; }
    public float RefundFraction { get; set; }   // per Wisp
    public float RefundCap { get; set; }        // total across overlapping Wisps
    public bool PauseDrain { get; set; }
    public float ReplenishPerSecond { get; set; }

    private float progress;

    private void OnEnable()  { all.Add(this); }
    private void OnDisable() { all.Remove(this); }

    protected override void Shoot() { }

    protected override void Update()
    {
        if (IsStunned) return;

        // lifetime auras: other Wisps are never affected
        if (PauseDrain || ReplenishPerSecond > 0f)
        {
            float dt = Time.deltaTime;
            foreach (Ghost g in Ghost.Active)
            {
                if (!InRange(g) || g is Wisp) continue;
                if (g is Yokai && !stallYokai) continue;

                if (PauseDrain) g.PauseNaturalDrain(0.25f); // refreshed every frame, so it lapses fast when they leave
                if (ReplenishPerSecond > 0f) g.AddLifetime(ReplenishPerSecond * dt);
            }
        }

        // passive income, no animation involved
        progress += HasteDelta;
        if (progress >= attackSpeed)
        {
            progress = 0f;
            Pulse();
        }
    }

    private void Pulse()
    {
        int count = 0;
        foreach (Ghost g in Ghost.Active)
            if (InRange(g)) count++;

        if (count <= 0) return;

        int perGhost = Mathf.RoundToInt((ectoPerGhost + BonusEctoPerGhost) * EctoMultiplier);
        LevelManager.main.IncreaseEcto(perGhost * count);

        if (pulseEffectPrefab != null)
        {
            GameObject fx = Instantiate(pulseEffectPrefab, transform.position, Quaternion.identity);
            fx.transform.localScale = Vector3.one * TargetingRange * 2f; // assumes a 1-unit sprite
        }
    }

    // other living ghosts inside the radius
    private bool InRange(Ghost g)
    {
        if (g == this || !g.IsAlive) return false;
        return Vector2.Distance(g.transform.position, transform.position) <= TargetingRange;
    }

    // 1-1 / 1-3: ecto when this Wisp expires, however it got there
    protected override void OnLifetimeExpired()
    {
        if (ExpireEcto > 0) LevelManager.main.IncreaseEcto(ExpireEcto);
        base.OnLifetimeExpired();
    }

    // 1-2: called by Ghost right before any ghost expires
    public static void NotifyGhostExpired(Ghost expired)
    {
        if (all.Count == 0 || expired == null) return;

        float fraction = 0f, cap = 0f;
        foreach (Wisp w in all)
        {
            if (w == expired || !w.IsAlive || w.RefundFraction <= 0f) continue;
            if (Vector2.Distance(w.transform.position, expired.transform.position) > w.TargetingRange) continue;

            fraction += w.RefundFraction;          // overlapping Wisps stack
            cap = Mathf.Max(cap, w.RefundCap);
        }
        if (fraction <= 0f) return;

        fraction = Mathf.Min(fraction, cap);

        LevelLoadout.Entry entry = LevelLoadout.entries.Find(e => e.tower.towerId == expired.TowerId);
        if (entry == null) return;

        LevelManager.main.IncreaseEcto(Mathf.RoundToInt(entry.tower.cost * fraction));
    }
}