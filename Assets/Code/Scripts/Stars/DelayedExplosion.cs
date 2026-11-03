using UnityEngine;

public class DelayedExplosion : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float fuseTime = 4f;

    [Header("Explosion")]
    [SerializeField] private float radius = 2f;
    [SerializeField] private float lifetimeDamage = 5f;  // seconds removed from ghosts
    [SerializeField] private float stunDuration = 0f;    // Hypernova: > 0

    [Header("Visuals")]
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private SpriteRenderer fuseTint;     // optional, pulses while fuse burns
    [SerializeField] private Color fuseColor = Color.red;

    private Health health;
    private bool fuseLit;
    private float fuseRemaining;
    private Color baseColor;

    public bool FuseLit => fuseLit;
    public float FuseFraction => fuseLit ? Mathf.Clamp01(fuseRemaining / fuseTime) : 1f;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (fuseTint != null) baseColor = fuseTint.color;
    }

    private void OnEnable()  { health.OnDamaged += HandleDamaged; }
    private void OnDisable() { health.OnDamaged -= HandleDamaged; }

    private void HandleDamaged(DamageInfo info)
    {
        if (fuseLit) return; // timer starts once and isn't reset
        fuseLit = true;
        fuseRemaining = fuseTime;
    }

    private void Update()
    {
        if (!fuseLit) return;

        fuseRemaining -= Time.deltaTime;

        if (fuseTint != null)
        {
            float pulse = Mathf.PingPong(Time.time * (2f + (1f - FuseFraction) * 6f), 1f);
            fuseTint.color = Color.Lerp(baseColor, fuseColor, pulse);
        }

        if (fuseRemaining <= 0f)
            Explode();
    }

    private void Explode()
    {
        fuseLit = false;
        enabled = false;

        foreach (Ghost g in Ghost.Active)
        {
            if (!g.IsAlive) continue;
            if (Vector2.Distance(g.transform.position, transform.position) > radius) continue;

            if (lifetimeDamage > 0f) g.ReduceLifetime(lifetimeDamage);
            if (stunDuration > 0f) g.Stun(stunDuration);
        }

        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // the star is consumed by its own explosion, so no death effects and no reward
        EnemySpawner.onEnemyDestroy.Invoke();
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}