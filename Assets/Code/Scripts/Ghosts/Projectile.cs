using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] protected float bulletSpeed = 4f;
    [SerializeField] protected float bulletLifetime = 2f;
    [SerializeField] protected float bulletDamage = 4f;
    [SerializeField] protected int pierce = 0;       // stars it passes through; it breaks on hit number pierce + 1
    [SerializeField] protected float turnRate = 0f;  // degrees per second, 0 = instant tracking

    public float SpeedMultiplier { get; set; } = 1f;
    public int Pierce { get => pierce; set => pierce = value; }
    public float TurnRate { get => turnRate; set => turnRate = value; }
    public float DamageMultiplier { get; set; } = 1f;

    protected Transform target;
    protected DamageType damageType = DamageType.Neutral;

    private Vector2 heading;
    private bool homing = true;
    private bool consumed;
    private int hitCount;
    private readonly HashSet<Health> alreadyHit = new HashSet<Health>();

    public void SetTarget(Transform t) { target = t; }
    public void SetDamageType(DamageType type) { damageType = type; }

    // fixed direction, no homing (Poltergeist pellets)
    public void SetDirection(Vector2 dir)
    {
        heading = dir.normalized;
        homing = false;
        rb.linearVelocity = heading * bulletSpeed * SpeedMultiplier;
    }

    // starts in a direction, then curves toward the target at the given turn rate
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
        Destroy(gameObject, bulletLifetime);
    }

    protected virtual void FixedUpdate()
    {
        if (consumed) return;

        // lose tracking if the target became untargetable (hidden protostar)
        if (target != null)
        {
            Health h = target.GetComponent<Health>();
            if (h != null && !h.IsTargetable) target = null;
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
        // no target: keep flying along the last heading

        Vector2 pull = Vector2.zero;
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

        rb.linearVelocity = heading * bulletSpeed * SpeedMultiplier + pull;
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

        ProjectileAbsorber absorber = other.GetComponentInParent<ProjectileAbsorber>();
        if (absorber != null && absorber.IsActive)
        {
            Absorbed(absorber);
            return;
        }

        Health h = other.GetComponentInParent<Health>();
        if (h == null || !h.IsTargetable) return; // pass through hidden stars

        OnHit(h);
    }

    protected virtual void OnHit(Health h)
    {
        if (!alreadyHit.Add(h)) return; // a star with several colliders only counts once

        h.TakeDamage(new DamageInfo(bulletDamage * DamageMultiplier, damageType, gameObject));
        hitCount++;

        if (hitCount > pierce)
        {
            consumed = true;
            Destroy(gameObject);
            return;
        }

        // passed through the star it was chasing, so fly on instead of circling back
        if (target != null && h.transform == target) target = null;
    }

    private static Vector2 Rotate(Vector2 v, float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }
}