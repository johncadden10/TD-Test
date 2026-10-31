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
    [SerializeField] protected int attackDamage = 10;
    [SerializeField] protected float preDeathDuration = 1f;

    [Header("Tier Visuals")]
    [SerializeField] private Color[] tierColors =
    {
        Color.white,                    // base
        Color.cyan,                     // tier 1
        Color.magenta,                  // tier 2
        new Color(1f, 0.5f, 0f)         // tier 3 (orange)
    };
    
    public int PathIndex { get; private set; }
    public int CurrentTier { get; private set; }
    public string TowerId { get; private set; }
    // stat multipliers modifiers can change
    public float DamageMultiplier { get; set; } = 1f;
    public float AttackSpeedMultiplier { get; set; } = 1f;
    public float RangeBonus { get; set; } = 0f;
    public int Tier { get; private set; }


    // events modifiers can subscribe to
    public event Action<Transform> OnFired;       // call inside Shoot logic
    public event Action OnTick;                   // call every Update

    private static readonly List<Ghost> active = new List<Ghost>();
    public static IReadOnlyList<Ghost> Active => active;

    private float stunUntil;
    private bool wasStunned;
    public bool IsStunned => Time.time < stunUntil;

    protected float lifetimeRemaining;
    private bool preDeathTriggered;
    public bool IsAlive => lifetimeRemaining > 0f;

    protected Transform target;
    protected float timeUntilFire;
    protected void RaiseFired(Transform t) => OnFired?.Invoke(t);

    public float TargetingRange => targetingRange;

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
        timeUntilFire += Time.deltaTime;
        
        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();

        if (!CheckTargetIsInRange())
        {
            target = null;
        }
        else
        {
            if (timeUntilFire >= attackSpeed && !isAttackAnimationPlaying)
            {
                if (towerAnimator != null)
                {
                    isAttackAnimationPlaying = true;
                    towerAnimator.SetAttacking(); // no argument needed
                    timeUntilFire = 0f;
                }
                else
                {
                    Shoot();
                    timeUntilFire = 0f;
                }

            }
        }
    }

    // each tower implements its own shoot logic
    protected abstract void Shoot();

    public void OnAttackFire()
    {
        if (target == null) return;
        Shoot();
    }

    public void OnAttackEnd()
    {
        isAttackAnimationPlaying = false;
    }

    private IEnumerator LifetimeCoroutine()
    {
        lifetimeRemaining = lifetime;

        while (lifetimeRemaining > 0f)
        {
            lifetimeRemaining -= Time.deltaTime;

            if (!preDeathTriggered && preDeathDuration > 0f && lifetimeRemaining <= preDeathDuration)
            {
                preDeathTriggered = true;
                OnPreDeath();
            }

            if (healthBar != null)
                healthBar.SetProgress(Mathf.Clamp01(lifetimeRemaining / lifetime));

            yield return null;
        }

        OnLifetimeExpired();
    }

    public void Stun(float duration)
    {
        stunUntil = Mathf.Max(stunUntil, Time.time + duration);
    }

    private void LateUpdate()
    {
        bool stunned = IsStunned;
        if (stunned == wasStunned) return;
        wasStunned = stunned;
        if (towerAnimator != null) towerAnimator.SetPaused(stunned);
    }

    protected virtual void OnLifetimeExpired()
    {
        enabled = false;
        if (towerAnimator != null)
        {
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

    public void OnDeathEnd()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        active.Remove(this);
    }

    public void ReduceLifetime(float seconds)
    {
        if (!IsAlive) return;
        lifetimeRemaining -= seconds; // the coroutine handles pre-death and expiry
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
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
            target = hits[0].transform;
    }

    protected bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= targetingRange;
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

    protected virtual void ApplyTierEffect(int pathIndex, int tierIndex) { }

    //private void OnDrawGizmosSelected()
    //{
    //    Handles.color = Color.cyan;
    //    Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
    //}
}
