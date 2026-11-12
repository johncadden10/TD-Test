using System.Collections.Generic;
using UnityEngine;

public class Banshee : Ghost
{
    [Header("Banshee References")]
    [SerializeField] private BansheeAttack bansheeEffect;
    [SerializeField] private GameObject pulseEffectPrefab;     // optional, scaled to the pulse radius
    [SerializeField] private GameObject killBlastEffectPrefab; // optional, scaled to the blast radius

    [Header("Pulse (tuning)")]
    [SerializeField] private float pulseInterval = 8f;
    [SerializeField] private float pulseRadiusMultiplier = 1.8f; // of the scream radius
    [SerializeField] private float pulseEffectDuration = 4f;     // slow, weaken and pierce boost

    // set by upgrade modifiers
    public bool PulseEnabled { get; set; }
    public bool PulseWeakens { get; set; }
    public float WeakenMultiplier { get; set; } = 0.5f;
    public int PulsePierceBonus { get; set; }
    public float OverlapMultiplier { get; set; } = 1f;
    public float KillBlastRadius { get; set; }
    public float KillBlastMultiplier { get; set; } = 1f;

    private float pulseTimer;

    private void OnEnable()  { Health.OnAnyStarKilled += HandleStarKilled; }
    private void OnDisable() { Health.OnAnyStarKilled -= HandleStarKilled; }

    protected override void Start()
    {
        base.Start();
        if (bansheeEffect != null)
            bansheeEffect.SetScaleMultiplier(RangeMultiplier);
    }

    protected override void Update()
    {
        if (IsStunned) return;

        float dt = HasteDelta;
        timeUntilFire += dt;

        bool inRange = Physics2D.OverlapCircle(transform.position, TargetingRange, enemyMask) != null;

        if (towerAnimator != null) towerAnimator.SetAttackingBool(inRange);
        if (bansheeEffect != null) bansheeEffect.SetActive(inRange);

        if (timeUntilFire >= attackSpeed && inRange)
        {
            Shoot();
            timeUntilFire = 0f;
        }

        if (PulseEnabled)
        {
            pulseTimer += dt;
            float pulseRadius = TargetingRange * pulseRadiusMultiplier;

            if (pulseTimer >= pulseInterval && Physics2D.OverlapCircle(transform.position, pulseRadius, enemyMask) != null)
            {
                pulseTimer = 0f;
                Pulse(pulseRadius);
            }
        }
    }

    // ---- scream ----
    protected override void Shoot()
    {
        foreach (Health h in StarsInRadius(TargetingRange))
        {
            float damage = ScaleVsStatus(attackDamage, h);       // flat bonus if the star has ANY status

            if (OverlapMultiplier > 1f && OverlapsAnotherBanshee(h.transform.position))
                damage *= OverlapMultiplier;

            h.TakeDamage(MakeDamage(damage));
        }
    }

    private bool OverlapsAnotherBanshee(Vector2 starPos)
    {
        foreach (Ghost g in Ghost.Active)
        {
            if (g == this || !g.IsAlive || g.IsStunned) continue;
            if (g is Banshee b && Vector2.Distance(starPos, b.transform.position) <= b.TargetingRange)
                return true;
        }
        return false;
    }

    // ---- pulse ----
    private void Pulse(float radius)
    {
        foreach (Health h in StarsInRadius(radius))
        {
            if (!h.IsTargetable) continue; // hidden Protostars stay unaffected

            // 1-3: temporary weaken
            if (PulseWeakens)
            {
                StatusController sc = h.GetComponent<StatusController>();
                if (sc != null) sc.AddWeaken(WeakenMultiplier, pulseEffectDuration);
            }

            // permanent belted strip
            BeltedModifier belted = h.GetComponent<BeltedModifier>();
            if (belted != null) belted.Strip();
        }

        // 1-2: ghosts in the pulse get extra pierce
        if (PulsePierceBonus > 0)
        {
            foreach (Ghost g in Ghost.Active)
            {
                if (g == this || !g.IsAlive) continue;
                if (Vector2.Distance(g.transform.position, transform.position) <= radius)
                    g.AmplifyPierce(PulsePierceBonus, pulseEffectDuration);
            }
        }

        if (pulseEffectPrefab != null)
        {
            GameObject fx = Instantiate(pulseEffectPrefab, transform.position, Quaternion.identity);
            fx.transform.localScale = Vector3.one * radius * 2f; // assumes a 1-unit sprite
        }
    }

    // ---- stars that die inside the scream radius explode ----
    private void HandleStarKilled(Health dead)
    {
        if (KillBlastRadius <= 0f || !IsAlive || dead == null) return;

        Vector2 pos = dead.transform.position;
        if (Vector2.Distance(pos, transform.position) > TargetingRange) return;

        float damage = attackDamage * KillBlastMultiplier;
        foreach (Health h in FindObjectsByType<Health>(FindObjectsSortMode.None))
        {
            if (h == dead) continue;
            if (Vector2.Distance(h.transform.position, pos) <= KillBlastRadius)
                h.TakeDamage(MakeDamage(damage));
        }

        if (killBlastEffectPrefab != null)
        {
            GameObject fx = Instantiate(killBlastEffectPrefab, pos, Quaternion.identity);
            fx.transform.localScale = Vector3.one * KillBlastRadius * 2f;
        }
    }

    private List<Health> StarsInRadius(float radius)
    {
        var result = new List<Health>();
        var seen = new HashSet<Health>();

        foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, radius, enemyMask))
        {
            Health h = c.GetComponentInParent<Health>();
            if (h != null && seen.Add(h)) result.Add(h);
        }
        return result;
    }
}