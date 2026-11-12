using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour, IDamageModifier
{
    [Header("Immunities")]
    [SerializeField] private bool immuneToColdSlow; // blocks chill from cold damage and cold slows

    [Header("Fire -> Burn")]
    [SerializeField] private float burnDps = 3f;
    [SerializeField] private float burnDuration = 3f;
    [SerializeField] private float burnTickInterval = 1f;

    [Header("Cold -> Chill")]
    [SerializeField] private float chillMultiplier = 0.7f;
    [SerializeField] private float chillDuration = 3f;

    private class SpeedEffect { public float multiplier; public float timeLeft; public bool cold; }
    private readonly List<SpeedEffect> speedEffects = new List<SpeedEffect>();

    private Health health;

    private float burnTimeLeft, burnCurrentDps, burnTickTimer;
    private float vulnerableTimeLeft, vulnerableMultiplier = 1f;
    private float weakenTimeLeft, weakenMultiplier = 1f;

    public float StunTimeLeft { get; private set; }

    public bool IsStunned => StunTimeLeft > 0f;
    public bool IsBurning => burnTimeLeft > 0f;
    public bool IsVulnerable => vulnerableTimeLeft > 0f;
    public bool IsSlowed => speedEffects.Count > 0;
    public bool IsWeakened => weakenTimeLeft > 0f;
    public float DamageDealtMultiplier => IsWeakened ? weakenMultiplier : 1f;
    public bool HasAnyStatus => IsStunned || IsBurning || IsVulnerable || IsSlowed || IsWeakened;

    public float SpeedMultiplier
    {
        get
        {
            if (IsStunned) return 0f;
            float m = 1f;
            foreach (SpeedEffect e in speedEffects) m = Mathf.Min(m, e.multiplier); // strongest slow wins
            return m;
        }
    }

    // vulnerability multiplies damage after resistances, before Fusion and Belted
    public int Order => 15;

    public void Modify(ref DamageInfo info)
    {
        if (IsVulnerable) info.amount *= vulnerableMultiplier;
    }

    private void Awake() { health = GetComponent<Health>(); }
    private void OnEnable()  { if (health != null) health.OnDamaged += HandleDamaged; }
    private void OnDisable() { if (health != null) health.OnDamaged -= HandleDamaged; }

    // damage types leave statuses behind
    private void HandleDamaged(DamageInfo info)
    {
        if (info.fromStatus) return;

        if (info.type.Has(DamageType.Fire))
            ApplyBurn(burnDps, burnDuration);

        if (info.type.Has(DamageType.Cold))
            AddSlow(chillMultiplier, chillDuration, true);
    }

    // ---- applying statuses ----
    public void ApplyBurn(float dps, float duration)
    {
        burnCurrentDps = IsBurning ? Mathf.Max(burnCurrentDps, dps) : dps; // strongest wins, duration refreshes
        burnTimeLeft = Mathf.Max(burnTimeLeft, duration);
    }

    public void AddSlow(float multiplier, float duration, bool isCold = false)
    {
        if (isCold && immuneToColdSlow) return;

        // same kind refreshes instead of piling up
        foreach (SpeedEffect e in speedEffects)
        {
            if (e.cold != isCold) continue;
            e.multiplier = Mathf.Min(e.multiplier, multiplier);
            e.timeLeft = Mathf.Max(e.timeLeft, duration);
            return;
        }
        speedEffects.Add(new SpeedEffect { multiplier = multiplier, timeLeft = duration, cold = isCold });
    }

    public void AddVulnerable(float multiplier, float duration)
    {
        vulnerableMultiplier = IsVulnerable ? Mathf.Max(vulnerableMultiplier, multiplier) : multiplier;
        vulnerableTimeLeft = Mathf.Max(vulnerableTimeLeft, duration);
    }

    public void AddWeaken(float multiplier, float duration)
    {
        weakenMultiplier = IsWeakened ? Mathf.Min(weakenMultiplier, multiplier) : multiplier;
        weakenTimeLeft = Mathf.Max(weakenTimeLeft, duration);
    }

    public void Stun(float duration) => StunTimeLeft = Mathf.Max(StunTimeLeft, duration);

    // ---- ticking ----
    private void Update()
    {
        float dt = Time.deltaTime;

        StunTimeLeft = Mathf.Max(0f, StunTimeLeft - dt);
        vulnerableTimeLeft = Mathf.Max(0f, vulnerableTimeLeft - dt);
        weakenTimeLeft = Mathf.Max(0f, weakenTimeLeft - dt);

        for (int i = speedEffects.Count - 1; i >= 0; i--)
        {
            speedEffects[i].timeLeft -= dt;
            if (speedEffects[i].timeLeft <= 0f) speedEffects.RemoveAt(i);
        }

        if (burnTimeLeft > 0f)
        {
            burnTimeLeft -= dt;
            burnTickTimer += dt;

            while (burnTickTimer >= burnTickInterval)
            {
                burnTickTimer -= burnTickInterval;
                if (health != null)
                    health.TakeDamage(new DamageInfo(burnCurrentDps * burnTickInterval, DamageType.Fire, null, true));
            }

            if (burnTimeLeft <= 0f) burnTickTimer = 0f;
        }
    }
}