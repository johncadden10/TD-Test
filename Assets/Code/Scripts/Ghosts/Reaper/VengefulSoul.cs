using UnityEngine;

[System.Serializable]
public class VengefulSettings
{
    public bool enabled;
    public GameObject prefab;
    public float baseDamage = 40f;
    public float damageMultiplier = 1f;
    public float splashRadius = 0f; // 0 = single target
    public DamageType damageType = DamageType.Neutral;
    public LayerMask enemyMask;
}

public class VengefulSoul : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float hitDistance = 0.25f;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;        // single-target impact
    [SerializeField] private GameObject explosionEffectPrefab;  // used when splash is active (path 1-2)
    [SerializeField] private float explosionBaseDiameter = 1f;  // world-unit width of the explosion art at scale 1
    [SerializeField] private float effectLifetime = 2f;         // safety cleanup for spawned effects

    private VengefulSettings settings;
    private Transform target;
    private float damage;

    public void Init(VengefulSettings s)
    {
        settings = s;
        damage = s.baseDamage * s.damageMultiplier;
        target = FindStrongest();
    }

    private void Update()
    {
        if (ProjectileAbsorber.TryAbsorbAt(transform.position))
        {
            Destroy(gameObject);
            return;
        }
        if (target != null && !target.GetComponent<Health>().IsTargetable)
            target = null;

        if (target == null)
        {
            target = FindStrongest();
            if (target == null) { Destroy(gameObject); return; }
        }

        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) <= hitDistance)
            Hit();
    }

    private void Hit()
    {
        if (settings.splashRadius > 0f)
        {
            foreach (Health h in FindObjectsByType<Health>(FindObjectsSortMode.None))
                if (Vector2.Distance(h.transform.position, transform.position) <= settings.splashRadius)
                    h.TakeDamage(new DamageInfo(damage, settings.damageType, gameObject));

            SpawnEffect(explosionEffectPrefab, settings.splashRadius * 2f / explosionBaseDiameter);
        }
        else
        {
            Health h = target != null ? target.GetComponentInParent<Health>() : null;
            if (h != null) h.TakeDamage(new DamageInfo(damage, settings.damageType, gameObject));

            SpawnEffect(hitEffectPrefab, 1f);
        }

        Destroy(gameObject);
    }

    private void SpawnEffect(GameObject prefab, float scale)
    {
        if (prefab == null) return;

        GameObject fx = Instantiate(prefab, transform.position, Quaternion.identity);
        fx.transform.localScale = Vector3.one * scale;
        Destroy(fx, effectLifetime);
    }

    private Transform FindStrongest()
    {
        Health best = null;
        float bestRemaining = float.MaxValue;

        foreach (Health h in FindObjectsByType<Health>(FindObjectsSortMode.None))
        {
            if (!h.IsTargetable) continue;

            EnemyMovement m = h.GetComponent<EnemyMovement>();
            float remaining = m != null ? m.DistanceRemaining : float.MaxValue;

            bool stronger = best == null || h.HitPoints > best.HitPoints + 0.001f;
            bool tied = best != null && Mathf.Abs(h.HitPoints - best.HitPoints) <= 0.001f;

            if (stronger || (tied && remaining < bestRemaining))
            {
                best = h;
                bestRemaining = remaining;
            }
        }

        return best != null ? best.transform : null;
    }
}