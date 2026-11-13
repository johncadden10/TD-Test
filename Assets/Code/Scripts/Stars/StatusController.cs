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
    private float permChillMultiplier = 1f;

    public float StunTimeLeft { get; private set; }

    public bool IsStunned => StunTimeLeft > 0f;
    public bool IsBurning => burnTimeLeft > 0f;
    public bool IsVulnerable => vulnerableTimeLeft > 0f;
    public bool IsSlowed => speedEffects.Count > 0 || IsPermanentlyChilled;
    public bool IsPermanentlyChilled => permChillMultiplier < 1f;
    public bool IsWeakened => weakenTimeLeft > 0f;
    public float DamageDealtMultiplier => IsWeakened ? weakenMultiplier : 1f;
    public bool HasAnyStatus => IsStunned || IsBurning || IsVulnerable || IsSlowed || IsWeakened;

    public float SpeedMultiplier
    {
        get
        {
            if (IsStunned) return 0f;
            float m = permChillMultiplier;
            foreach (SpeedEffect e in speedEffects) m = Mathf.Min(m, e.multiplier);
            return m;
        }
    }

    public bool IsChilled
    {
        get
        {
            if (IsPermanentlyChilled) return true;
            foreach (SpeedEffect e in speedEffects) if (e.cold) return true;
            return false;
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

    private bool CanReceiveDirectly()
    {
        ConstellationLink link = GetComponent<ConstellationLink>();
        return link == null || !link.IsBlocked;
    }

    // ---- applying statuses ----
    private void ApplyBurnLocal(float dps, float duration)
    {
        burnCurrentDps = IsBurning ? Mathf.Max(burnCurrentDps, dps) : dps; // strongest wins, duration refreshes
        burnTimeLeft = Mathf.Max(burnTimeLeft, duration);
    }

    private void AddSlowLocal(float multiplier, float duration, bool isCold = false)
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

    private void AddVulnerableLocal(float multiplier, float duration)
    {
        vulnerableMultiplier = IsVulnerable ? Mathf.Max(vulnerableMultiplier, multiplier) : multiplier;
        vulnerableTimeLeft = Mathf.Max(vulnerableTimeLeft, duration);
    }

    private void AddWeakenLocal(float multiplier, float duration)
    {
        weakenMultiplier = IsWeakened ? Mathf.Min(weakenMultiplier, multiplier) : multiplier;
        weakenTimeLeft = Mathf.Max(weakenTimeLeft, duration);
    }

    private void AddPermanentChillLocal(float multiplier)
    {
        if (immuneToColdSlow) return;
        permChillMultiplier = Mathf.Min(permChillMultiplier, multiplier);
    }

    private void StunLocal(float duration) => StunTimeLeft = Mathf.Max(StunTimeLeft, duration);

    // constellation appliers
    private void Share(System.Action<StatusController> apply)
    {
        ConstellationLink link = GetComponent<ConstellationLink>();
        if (link == null) return;
        foreach (StatusController s in link.Followers()) apply(s);
    }

    public void Stun(float duration)
    {
        if (!CanReceiveDirectly()) return;
        StunLocal(duration);
        Share(s => s.StunLocal(duration));
    }

    public void AddSlow(float multiplier, float duration, bool isCold = false)
    {
        if (!CanReceiveDirectly()) return;
        AddSlowLocal(multiplier, duration, isCold);
        Share(s => s.AddSlowLocal(multiplier, duration, isCold));
    }

    public void AddPermanentChill(float multiplier)
    {
        if (!CanReceiveDirectly()) return;
        AddPermanentChillLocal(multiplier);
        Share(s => s.AddPermanentChillLocal(multiplier));
    }

    public void ApplyBurn(float dps, float duration)
    {
        if (!CanReceiveDirectly()) return;
        ApplyBurnLocal(dps, duration);
        Share(s => s.ApplyBurnLocal(dps, duration));
    }

    public void AddVulnerable(float multiplier, float duration)
    {
        if (!CanReceiveDirectly()) return;
        AddVulnerableLocal(multiplier, duration);
        Share(s => s.AddVulnerableLocal(multiplier, duration));
    }

    public void AddWeaken(float multiplier, float duration)
    {
        if (!CanReceiveDirectly()) return;
        AddWeakenLocal(multiplier, duration);
        Share(s => s.AddWeakenLocal(multiplier, duration));
    }

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