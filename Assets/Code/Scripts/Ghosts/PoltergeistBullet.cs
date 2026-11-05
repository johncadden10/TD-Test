using UnityEngine;

public class PoltergeistBullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 6f;
    [SerializeField] private float bulletLifetime = 2.5f;
    [SerializeField] private float bulletDamage = 4f;
    
    private Transform target;
    //private float timeLeft;
    private DamageType damageType = DamageType.Neutral;

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void SetDamageType(DamageType type)
    {
        damageType = type;
    }

    private void Start()
    {
        //timeLeft = bulletLifetime;
        Destroy(gameObject, bulletLifetime);
    }

    private void Update()
    {
        //timeLeft -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Health h = target.GetComponent<Health>();
            if (h != null && !h.IsTargetable) target = null; // lose tracking
        }

        if (!target) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Health h = other.gameObject.GetComponentInParent<Health>();
        if (h != null) h.TakeDamage(new DamageInfo(bulletDamage, damageType, gameObject));
        if (h != null && !h.IsTargetable) return;
        other.gameObject.GetComponent<Health>().TakeDamage(bulletDamage);
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 direction)
    {
        rb.linearVelocity = direction * bulletSpeed;
    }
}
