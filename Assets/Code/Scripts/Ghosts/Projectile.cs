using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private GameObject blastEffectPrefab; // optional, scaled to the blast radius

    [Header("Attributes")]
    [SerializeField] protected float bulletSpeed = 4f;
    [SerializeField] protected float bulletLifetime = 2f;
    [SerializeField] protected float bulletDamage = 4f;
    [SerializeField] protected int pierce = 0;
    [SerializeField] protected float turnRate = 0f;

    public float SpeedMultiplier { get; set; } = 1f;
    public int Pierce { get => pierce; set => pierce = value; }
    public float TurnRate { get => turnRate; set => turnRate = value; }
    public bool IgnoreBelted { get; set; }
    public bool IgnoreAbsorbers { get; set; }

    public bool Reseek { get; set; }
    private float retargetTimer;

    protected Transform target;
    protected DamageType damageType = DamageType.Neutral;

    private Vector2 heading;
    private bool homing = true;
    private bool consumed;
    private int hitCount;
    private readonly HashSet<Health> alreadyHit = new HashSet<Health>();

    // delayed blast
    private float blastDelay, blastRadius, blastDamageMultiplier;
    private bool fusing;
    private Transform stuckTo;
    private Vector3 stuckOffset;

    public void SetTarget(Transform t) { target = t; }
    public void SetDamageType(DamageType type) { damageType = type; }

    public void ConfigureBlast(float delay, float radius, float damageMultiplier)
    {
        blastDelay = delay;
        blastRadius = radius;
        blastDamageMultiplier = damageMultiplier;
    }

    public void SetDirection(Vector2 dir)
    {
        heading = dir.normalized;
        homing = false;
        rb.linearVelocity = heading * bulletSpeed * SpeedMultiplier;
    }

    public void Launch(Vector2 dir, Transform t, float turn)
    {
        heading = dir.normalized;
        target = t;
        turnRate = turn;
        homing = true;
        rb.linearVelocity = heading * bulletSpeed * SpeedMultiplier;
    }

    protected virtual void Start()
    {
        Invoke(nameof(Expire), bulletLifetime);
    }

    private void Expire()
    {
        if (!fusing) Destroy(gameObject);
    }

    private void Update()
    {
        if (fusing && stuckTo != null)
            transform.position = stuckTo.position + stuckOffset; // rides the star while the fuse burns
    }

    protected virtual void FixedUpdate()
    {
        if (consumed) return;

        if (target != null)
        {
            Health h = target.GetComponent<Health>();
            if (h != null && !h.IsTargetable) target = null;
        }

        if (Reseek && homing && target == null)
        {
            retargetTimer -= Time.fixedDeltaTime;
            if (retargetTimer <= 0f)
            {
                retargetTimer = 0.1f;
                target = FindNewTarget();
            }
        }

        if (homing && target != null)
        {
            Vector2 desired = ((Vector2)target.position - rb.position).normalized;

            if (heading == Vector2.zero || turnRate <= 0f)
            {
                heading = desired;
            }
            else
            {
                float maxStep = turnRate * Time.fixedDeltaTime;
                float step = Mathf.Clamp(Vector2.SignedAngle(heading, desired), -maxStep, maxStep);
                heading = Rotate(heading, step);
            }
        }

        Vector2 pull = Vector2.zero;
        if (!IgnoreAbsorbers)
        {
            foreach (ProjectileAbsorber a in ProjectileAbsorber.All)
            {
                if (!a.IsActive) continue;

                if (a.InsideHorizon(rb.position))
                {
                    Absorbed(a);
                    return;
                }
                pull += a.PullAt(rb.position);
            }
        }

        rb.linearVelocity = heading * bulletSpeed * SpeedMultiplier + pull;
    }

    private Transform FindNewTarget()
    {
        Transform best = null;
        float bestDist = float.MaxValue;

        foreach (Health h in FindObjectsByType<Health>(FindObjectsSortMode.None))
        {
            if (!h.IsTargetable || alreadyHit.Contains(h)) continue;

            ConstellationLink link = h.GetComponent<ConstellationLink>();
            if (link != null && link.IsBlocked) continue;

            float d = Vector2.Distance(rb.position, h.transform.position);
            if (d < bestDist) { bestDist = d; best = h.transform; }
        }
        return best;
    }

    private void Absorbed(ProjectileAbsorber a)
    {
        consumed = true;
        a.OnAbsorbed(transform.position);
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (consumed) return;

        if (!IgnoreAbsorbers)
        {
            ProjectileAbsorber absorber = other.GetComponentInParent<ProjectileAbsorber>();
            if (absorber != null && absorber.IsActive)
            {
                Absorbed(absorber);
                return;
            }
        }

        Health h = other.GetComponentInParent<Health>();
        if (h == null || !h.IsTargetable) return;

        OnHit(h);
    }

    protected virtual void OnHit(Health h)
    {
        if (!alreadyHit.Add(h)) return;

        h.TakeDamage(MakeDamage(bulletDamage));
        hitCount++;

        if (hitCount > pierce)
        {
            consumed = true;
            if (blastDelay > 0f) BeginFuse(h.transform);
            else Destroy(gameObject);
            return;
        }

        if (target != null && h.transform == target) target = null;
    }

    private void BeginFuse(Transform star)
    {
        CancelInvoke(nameof(Expire));
        fusing = true;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false; // no more collisions
        stuckTo = star;
        stuckOffset = transform.position - star.position;
        Invoke(nameof(Blast), blastDelay);
    }

    private void Blast()
    {
        Vector2 pos = transform.position;
        float damage = bulletDamage * blastDamageMultiplier;

        foreach (Health h in FindObjectsByType<Health>(FindObjectsSortMode.None))
        {
            if (Vector2.Distance(h.transform.position, pos) > blastRadius) continue;
            h.TakeDamage(MakeDamage(damage));
        }

        if (blastEffectPrefab != null)
        {
            GameObject fx = Instantiate(blastEffectPrefab, pos, Quaternion.identity);
            fx.transform.localScale = Vector3.one * blastRadius * 2f; // assumes a 1-unit sprite
        }

        Destroy(gameObject);
    }

    private DamageInfo MakeDamage(float amount)
    {
        DamageInfo info = new DamageInfo(amount, damageType, gameObject);
        info.ignoreBelted = IgnoreBelted;
        return info;
    }

    private static Vector2 Rotate(Vector2 v, float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }
}