using UnityEngine;

[System.Serializable]
public class VengefulSettings
{
    public bool enabled;
    public GameObject prefab;
    public float baseDamage = 40f;
    public float damageMultiplier = 1f;
    public float splashRadius = 0f; // 0 = single target
    public LayerMask enemyMask;
}

public class VengefulSoul : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float hitDistance = 0.25f;

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
        Debug.Log($"Vengeful hit, damage {damage}, splash {settings.splashRadius}");

        if (settings.splashRadius > 0f)
        {
            foreach (Health h in FindObjectsByType<Health>(FindObjectsSortMode.None))
                if (Vector2.Distance(h.transform.position, transform.position) <= settings.splashRadius)
                    h.TakeDamage(damage);
        }
        else
        {
            Health h = target.GetComponentInParent<Health>();
            if (h != null) h.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    private Transform FindStrongest()
    {
        Health best = null;
        foreach (Health h in FindObjectsByType<Health>(FindObjectsSortMode.None))
        {
            if (!h.IsTargetable) continue;
            if (best == null || h.HitPoints > best.HitPoints) best = h;
        }
        return best != null ? best.transform : null;
    }
}