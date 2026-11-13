using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class Ghost : MonoBehaviour
{
    [Header("Base References")]
    [SerializeField] protected TowerAnimator towerAnimator;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected Transform firingPoint;
    [SerializeField] protected TowerHealthBar healthBar;

    [Header("Base Attributes")]
    [SerializeField] protected float targetingRange = 5f;
    [SerializeField] protected float attackSpeed = 1f;
    [SerializeField] protected float lifetime = 16f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float preDeathDuration = 1f;
    [SerializeField] private float retargetInterval = 0.1f;

    [Header("Damage Type")]
    [SerializeField] private DamageType damageType = DamageType.Neutral;

    [Header("Tier Visuals")]
    [SerializeField] private Color[] tierColors =
    {
        Color.white,                    // base
        Color.cyan,                     // tier 1
        Color.magenta,                  // tier 2
        new Color(1f, 0.5f, 0f)         // tier 3 (orange)
    };

    [Header("Phase Visuals")]
    [SerializeField, Range(0.1f, 1f)] private float phasedOpacity = 0.5f;
    
    public int PathIndex { get; private set; }
    public int CurrentTier { get; private set; }
    public string TowerId { get; private set; }
    // stat multipliers modifiers can change
    public float DamageMultiplier { get; set; } = 1f;
    public float RangeBonus { get; set; } = 0f;
    public int Tier { get; private set; }
    public float LifetimeMultiplier { get; set; } = 1f;
    public float MaxLifetime => lifetime * LifetimeMultiplier;
    
    // range upgrades (Yokai uses this as its blast radius)
    public float RangeMultiplier { get; set; } = 1f;
    public float TargetingRange => targetingRange * RangeMultiplier;

    // restoring lifetime, with optional overheal
    private float overhealHaste = 1f;
    public bool IsOverhealed => lifetimeRemaining > MaxLifetime;
    public float FireRateMultiplier => FireRateBonus * (IsOverhealed ? overhealHaste : 1f);
    public float FireRateBonus { get; set; } = 1f;

    public DamageType DamageType
    {
        get => damageType;
        set => damageType = value;
    }


    // events modifiers can subscribe to
    public event Action<Transform> OnFired;       // call inside Shoot logic
    public event Action OnTick;                   // call every Update
    public event System.Action<float> OnLifetimeReduced;

    private static readonly List<Ghost> active = new List<Ghost>();
    public static IReadOnlyList<Ghost> Active => active;

    private float stunUntil;
    private bool wasStunned;
    public bool IsStunned => Time.time < stunUntil;

    public bool PhaseOnAttack { get; set; }
    public float PhaseDuration { get; set; } = 3f;
    public float PhaseCooldown { get; set; } = 5f;
    public float PhaseLifetimeRegen { get; set; }
    public float PhaseShareRadius { get; set; } 
    public float PhaseShareRegenFraction { get; set; }
    public float PhaseRegenMultiplier { get; set; } = 1f;
    public bool IsPhased => Time.time < phaseUntil;

    public float StatusBonusMultiplier { get; set; } = 1f;

    public float OverhealCapSeconds { get; private set; }
    protected float HasteDelta => Time.deltaTime * FireRateMultiplier;

    protected float lifetimeRemaining;
    private bool preDeathTriggered;
    public bool IsAlive => lifetimeRemaining > 0f;

    protected Transform target;
    protected float timeUntilFire;
    protected void RaiseFired(Transform t) => OnFired?.Invoke(t);

    private float phaseUntil;
    private float nextPhaseAllowed;
    private float phaseAlpha = 1f;

    private int tempPierce;
    private float tempPierceUntil;
    public int TempPierce => Time.time < tempPierceUntil ? tempPierce : 0;

    private float retargetTimer;
    private bool isDying;
    private bool isAttackAnimationPlaying = false;
    private List<GhostModifier> modifiers = new List<GhostModifier>();

    protected virtual void Start()
    {
        towerAnimator = GetComponent<TowerAnimator>();
        active.Add(this);
        StartCoroutine(LifetimeCoroutine());
    }

    protected virtual void Update()
    {
        if (IsStunned) return;

        if (target != null && !IsValidTarget(target)) target = null;

        retargetTimer -= Time.deltaTime;
        if (retargetTimer <= 0f)
        {
            retargetTimer = retargetInterval;
            FindTarget();
        }

        if (target == null) return;

        RotateTowardsTarget();

        if (!CheckTargetIsInRange())
        {
            target = null;
            return;
        }

        if (!isAttackAnimationPlaying)
            timeUntilFire += HasteDelta;

        if (timeUntilFire >= attackSpeed && !isAttackAnimationPlaying)
        {
            if (towerAnimator != null)
            {
                isAttackAnimationPlaying = true;
                towerAnimator.SetAttacking();
            }
            else
            {
                Shoot();
            }
            timeUntilFire = 0f;
        }
    }

    // each tower implements its own shoot logic
    protected abstract void Shoot();

    public virtual void OnAttackFire()
    {
        if (target == null) return;
        Shoot();
    }

    public virtual void OnAttackEnd()
    {
        isAttackAnimationPlaying = false;
    }

    private IEnumerator LifetimeCoroutine()
    {
        lifetimeRemaining = MaxLifetime;

        while (lifetimeRemaining > 0f)
        {
            lifetimeRemaining -= Time.deltaTime;

            if (!preDeathTriggered && preDeathDuration > 0f && lifetimeRemaining <= preDeathDuration)
            {
                preDeathTriggered = true;
                OnPreDeath();
            }

            if (healthBar != null) 
                healthBar.SetLifetime(lifetimeRemaining, MaxLifetime, OverhealCapSeconds);

            yield return null;
        }

        OnLifetimeExpired();
    }

    public void Stun(float duration)
    {
        if (isDying) return;
        if (AbsorbStarAttack()) return;
        stunUntil = Mathf.Max(stunUntil, Time.time + duration);
    }

    private void LateUpdate()
    {
        bool stunned = IsStunned;
        if (stunned != wasStunned)
        {
            wasStunned = stunned;
            if (towerAnimator != null) towerAnimator.SetPaused(stunned);
        }
        if (towerAnimator != null)
            towerAnimator.SetSpeedMultiplier(FireRateMultiplier);

        float targetAlpha = IsPhased ? phasedOpacity : 1f;
        if (!Mathf.Approximately(phaseAlpha, targetAlpha))
        {
            phaseAlpha = Mathf.MoveTowards(phaseAlpha, targetAlpha, 6f * Time.deltaTime);
            SetSpriteAlpha(phaseAlpha);
        }
    }

    protected virtual void OnLifetimeExpired()
    {
        isDying = true;

        // clear any stun so the death animation can actually play
        stunUntil = 0f;
        wasStunned = false;
        SetSpriteAlpha(1f);

        enabled = false;

        if (towerAnimator != null)
        {
            towerAnimator.SetPaused(false); // must come before SetDying
            towerAnimator.SetDying();
        }
        else
        {
            Destroy(gameObject); // fallback if no animator
        }
    }

    protected virtual void OnPreDeath()
    {
        if (towerAnimator != null)
            towerAnimator.SetPreDeath();
    }

    public virtual void OnDeathEnd()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        active.Remove(this);
    }

    private bool AbsorbStarAttack()
    {
        if (IsPhased) return true;                         // phased (by itself or a Phantom): immune
        if (!PhaseOnAttack) return false;
        if (Time.time < nextPhaseAllowed) return false;

        phaseUntil = Time.time + PhaseDuration;
        nextPhaseAllowed = phaseUntil + PhaseCooldown;
        AddLifetime(PhaseLifetimeRegen);
        SharePhase();
        return true;                                       // the hit that triggered it does nothing
    }

    private void SharePhase()
    {
        if (PhaseShareRadius <= 0f) return;

        foreach (Ghost g in active)
        {
            if (g == this || !g.IsAlive) continue;
            if (Vector2.Distance(g.transform.position, transform.position) > PhaseShareRadius) continue;

            g.ForcePhase(PhaseDuration);
            g.AddLifetime(PhaseLifetimeRegen * PhaseShareRegenFraction);
        }
    }

    // phase without needing to be attacked: no cooldown, and it never triggers sharing itself
    public void ForcePhase(float duration)
    {
        if (isDying) return;
        phaseUntil = Mathf.Max(phaseUntil, Time.time + duration);
    }

    public void ReduceLifetime(float seconds)
    {
        if (!IsAlive || isDying) return;
        if (AbsorbStarAttack()) return;
        lifetimeRemaining -= seconds;
        if (seconds > 0f) OnLifetimeReduced?.Invoke(seconds);
    }

    public void AddLifetime(float seconds)
    {
        if (!IsAlive || isDying || seconds <= 0f) return;
        float raised = Mathf.Min(lifetimeRemaining + seconds, MaxLifetime);
        lifetimeRemaining = Mathf.Max(lifetimeRemaining, raised);
    }

    public void RestoreLifetime(float seconds, float overhealSeconds, float haste)
    {
        if (isDying || !IsAlive) return;

        OverhealCapSeconds = Mathf.Max(OverhealCapSeconds, overhealSeconds);

        float cap = Mathf.Max(lifetimeRemaining, MaxLifetime + overhealSeconds);
        lifetimeRemaining = Mathf.Min(lifetimeRemaining + seconds, cap);

        if (haste > 1f) overhealHaste = Mathf.Max(overhealHaste, haste);
    }

    public static Ghost Nearest(Vector2 from, float maxRange)
    {
        Ghost best = null;
        float bestDist = maxRange;
        foreach (Ghost g in active)
        {
            if (!g.IsAlive) continue;
            float d = Vector2.Distance(from, g.transform.position);
            if (d <= bestDist) { bestDist = d; best = g; }
        }
        return best;
    }

    protected void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, TargetingRange, Vector2.zero, 0f, enemyMask);

        Transform best = null;
        float bestRemaining = float.MaxValue;

        foreach (RaycastHit2D hit in hits)
        {
            if (!IsValidTarget(hit.transform)) continue;

            EnemyMovement m = hit.transform.GetComponentInParent<EnemyMovement>();
            float remaining = m != null ? m.DistanceRemaining : float.MaxValue;

            if (best == null || remaining < bestRemaining)
            {
                best = hit.transform;
                bestRemaining = remaining;
            }
        }

        target = best;
    }

    protected static bool IsValidTarget(Transform t)
    {
        if (t == null) return false;
        Health h = t.GetComponent<Health>();
        return h != null && h.IsTargetable;
    }

    protected bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= TargetingRange;
    }

    protected virtual void RotateTowardsTarget()
    {
        transform.localScale = target.position.x > transform.position.x
            ? new Vector3(-1f, 1f, 1f)
            : new Vector3(1f, 1f, 1f);
    }

    public void Initialize(string towerId, int pathIndex)
    {
        TowerId = towerId;
        PathIndex = pathIndex;

        TowerDefinition def = LevelLoadout.entries.Find(e => e.tower.towerId == towerId).tower;
        int tier = LevelUpgradeManager.main != null ? LevelUpgradeManager.main.GetTier(towerId) : 0;

        for (int i = 0; i < tier; i++)
        {
            GameObject prefab = def.upgradePaths[pathIndex].tiers[i].modifierPrefab;
            if (prefab == null) continue;

            GameObject obj = Instantiate(prefab, transform);
            foreach (GhostModifier m in obj.GetComponents<GhostModifier>())
            {
                m.Attach(this);
                modifiers.Add(m);
                ApplyTierColor(tier);
            }
        }
    }
    public virtual void ApplyUpgrade()
    {
        CurrentTier++;
    }

    private void ApplyTierColor(int tier)
    {
        Tier = tier;
        Color color = tierColors[Mathf.Clamp(tier, 0, tierColors.Length - 1)];

        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr.GetComponent<NoTierTint>() != null) continue; // skip excluded sprites
            sr.color = color;
        }
    }

    private void SetSpriteAlpha(float a)
    {
        foreach (SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr.GetComponent<NoTierTint>() != null) continue;
            Color c = sr.color;
            c.a = a;
            sr.color = c;
        }
    }

    protected DamageInfo MakeDamage(float amount)
    {
        return new DamageInfo(amount, damageType, gameObject);
    }

    protected float ScaleVsStatus(float amount, Component star)
    {
        StatusController sc = star.GetComponentInParent<StatusController>();
        return (sc != null && sc.HasAnyStatus) ? amount * StatusBonusMultiplier : amount;
    }

    public void AmplifyPierce(int amount, float duration)
    {
        if (Time.time >= tempPierceUntil) tempPierce = 0;
        tempPierce = Mathf.Max(tempPierce, amount);
        tempPierceUntil = Mathf.Max(tempPierceUntil, Time.time + duration);
    }

    protected virtual void ApplyTierEffect(int pathIndex, int tierIndex) { }

    //private void OnDrawGizmosSelected()
    //{
    //    Handles.color = Color.cyan;
    //    Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    //}
}
