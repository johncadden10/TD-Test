using UnityEngine;

public class FreezeGhostsOnDeath : MonoBehaviour, IDeathEffect
{
    [Header("Attributes")]
    [SerializeField] private float radius = 2.5f;
    [SerializeField] private float freezeDuration = 4f;
    [SerializeField] private bool anyFireHitDisables = false; // true = any fire hit earlier also cancels it

    [Header("Visuals")]
    [SerializeField] private GameObject effectPrefab;

    private Health health;
    private bool wasBurned;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()  { health.OnDamaged += HandleDamaged; }
    private void OnDisable() { health.OnDamaged -= HandleDamaged; }

    private void HandleDamaged(DamageInfo info)
    {
        if (info.type.Has(DamageType.Fire)) wasBurned = true;
    }

    public void OnDeath(DamageInfo killingBlow)
    {
        // fire disables the death effect
        if (killingBlow.type.Has(DamageType.Fire)) return;
        if (anyFireHitDisables && wasBurned) return;

        foreach (Ghost g in Ghost.Active)
        {
            if (!g.IsAlive) continue;
            if (Vector2.Distance(g.transform.position, transform.position) <= radius)
                g.Stun(freezeDuration);
        }

        if (effectPrefab != null)
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}