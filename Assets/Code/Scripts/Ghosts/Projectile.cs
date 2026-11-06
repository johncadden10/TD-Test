using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] protected float bulletSpeed = 4f;
    [SerializeField] protected float bulletLifetime = 2f;
    [SerializeField] protected float bulletDamage = 4f;

    protected Transform target;
    protected DamageType damageType = DamageType.Neutral;

    private Vector2 direction;
    private bool straight;
    private Vector2 baseVelocity;
    private bool consumed;

    public void SetTarget(Transform t) { target = t; }
    public void SetDamageType(DamageType type) { damageType = type; }

    // a fixed direction means no homing (Poltergeist pellets)
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        straight = true;
        baseVelocity = direction * bulletSpeed;
        rb.linearVelocity = baseVelocity;
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

        if (straight)
            baseVelocity = direction * bulletSpeed;
        else if (target != null)
            baseVelocity = ((Vector2)target.position - rb.position).normalized * bulletSpeed;
        // else: keep flying along the last velocity

        // black holes: absorb or bend the path
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

        rb.linearVelocity = baseVelocity + pull;
    }

    private void Absorbed(ProjectileAbsorber a)
    {
        consumed = true;
        a.OnAbsorbed(transform.position);
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (consumed) return;

        // safety net: touching a black hole directly counts as absorbed
        ProjectileAbsorber absorber = other.gameObject.GetComponentInParent<ProjectileAbsorber>();
        if (absorber != null && absorber.IsActive)
        {
            Absorbed(absorber);
            return;
        }

        Health h = other.gameObject.GetComponentInParent<Health>();
        if (h == null || !h.IsTargetable) return; // pass through hidden stars

        OnHit(h);
    }

    protected virtual void OnHit(Health h)
    {
        consumed = true;
        h.TakeDamage(new DamageInfo(bulletDamage, damageType, gameObject));
        Destroy(gameObject);
    }
}