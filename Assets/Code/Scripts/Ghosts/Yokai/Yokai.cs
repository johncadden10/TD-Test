using System.Collections.Generic;
using UnityEngine;

public class Yokai : Ghost
{
    [Header("Yokai References")]
    [SerializeField] private ExplosionEffect explosionEffectPrefab;
    [SerializeField] private FlameZone flameZonePrefab;

    [Header("Flames")]
    [SerializeField] private float flameDamagePerTick = 3f;
    [SerializeField] private float flameTickInterval = 0.5f;

    // set by upgrade modifiers
    public float ExplosionDamageMultiplier { get; set; } = 1f;
    public float FlameDuration { get; set; } = 0f;          // 0 = no flames
    public bool FlamesStun { get; set; } = false;
    public float LifetimeRestoreSeconds { get; set; } = 0f;
    public float OverhealSeconds { get; set; } = 0f;
    public float OverhealHaste { get; set; } = 1f;

    private bool detonating;

    private class Blast
    {
        public Vector2 position;
        public float radius, damage;
        public DamageType type;
        public LayerMask mask;
        public float restoreSeconds, overhealSeconds, haste;
        public float flameDuration, flameDamage, flameTick;
        public bool flameStun;
        public FlameZone flamePrefab;
    }

    // a Yokai never attacks on its own
    protected override void Update() { }
    protected override void Shoot() { }

    protected override void OnLifetimeExpired()
    {
        if (towerAnimator == null) { OnDeathEnd(); return; } // no animation to wait for
        base.OnLifetimeExpired(); // clears stun and plays PreDeath/Death
    }

    // called by the Death clip's last frame
    public override void OnDeathEnd()
    {
        if (detonating) return;
        detonating = true;

        Blast blast = new Blast
        {
            position = transform.position,
            radius = TargetingRange,
            damage = attackDamage * ExplosionDamageMultiplier,
            type = DamageType,
            mask = enemyMask,
            restoreSeconds = LifetimeRestoreSeconds,
            overhealSeconds = OverhealSeconds,
            haste = OverhealHaste,
            flameDuration = FlameDuration,
            flameDamage = flameDamagePerTick,
            flameTick = flameTickInterval,
            flameStun = FlamesStun,
            flamePrefab = flameZonePrefab
        };

        if (explosionEffectPrefab != null)
        {
            ExplosionEffect fx = Instantiate(explosionEffectPrefab, blast.position, Quaternion.identity);
            fx.Init(blast.radius, () => Detonate(blast));
        }
        else
        {
            Detonate(blast);
        }

        Destroy(gameObject);
    }

    // static: this Yokai is gone by the time the impact frame plays
    private static void Detonate(Blast b)
    {
        // stars
        var hit = new HashSet<Health>();
        foreach (Collider2D c in Physics2D.OverlapCircleAll(b.position, b.radius, b.mask))
        {
            Health h = c.GetComponentInParent<Health>();
            if (h == null || !hit.Add(h)) continue;

            // hidden Protostars are only hit if they're within their own reveal distance
            HiddenUnlessNearGhost hidden = h.GetComponent<HiddenUnlessNearGhost>();
            bool nearEnough = hidden != null && hidden.WithinRevealRange(b.position);

            h.TakeDamage(new DamageInfo(b.damage, b.type, null, false, nearEnough));
        }

        // ghosts
        if (b.restoreSeconds > 0f)
        {
            foreach (Ghost g in Ghost.Active)
            {
                if (Vector2.Distance(g.transform.position, b.position) > b.radius) continue;
                g.RestoreLifetime(b.restoreSeconds, b.overhealSeconds, b.haste);
            }
        }

        // flames
        if (b.flameDuration > 0f && b.flamePrefab != null)
        {
            FlameZone zone = Instantiate(b.flamePrefab, b.position, Quaternion.identity);
            zone.Init(b.radius, b.flameDuration, b.flameDamage, b.flameTick, b.mask, b.flameStun);
        }
    }
}