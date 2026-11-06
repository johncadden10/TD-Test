using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbsorber : MonoBehaviour
{
    private static readonly List<ProjectileAbsorber> all = new List<ProjectileAbsorber>();
    public static IReadOnlyList<ProjectileAbsorber> All => all;

    [Header("Attributes")]
    [SerializeField] private float horizonRadius = 1.5f; // projectiles inside this are destroyed
    [SerializeField] private float pullRadius = 3.5f;    // projectiles inside this curve inward
    [SerializeField] private float pullStrength = 4f;    // extra speed toward the center at the horizon

    [Header("Visuals")]
    [SerializeField] private GameObject absorbEffectPrefab;
    [SerializeField] private Transform horizonVisual;    // optional, scaled to the horizon

    private StatusController status;

    // a stunned black hole stops absorbing
    public bool IsActive => status == null || !status.IsStunned;

    private void Awake()
    {
        status = GetComponent<StatusController>();
        if (horizonVisual != null)
            horizonVisual.localScale = Vector3.one * horizonRadius * 2f; // assumes a 1-unit sprite
    }

    private void OnEnable()  { all.Add(this); }
    private void OnDisable() { all.Remove(this); }

    public bool InsideHorizon(Vector2 pos)
    {
        return Vector2.Distance(pos, transform.position) <= horizonRadius;
    }

    public Vector2 PullAt(Vector2 pos)
    {
        Vector2 toCenter = (Vector2)transform.position - pos;
        float d = toCenter.magnitude;
        if (d > pullRadius || d < 0.001f) return Vector2.zero;

        float strength = pullStrength * (1f - d / pullRadius);
        return toCenter / d * strength;
    }

    public void OnAbsorbed(Vector2 at)
    {
        if (absorbEffectPrefab != null)
            Instantiate(absorbEffectPrefab, at, Quaternion.identity);
    }

    // for projectiles that don't extend Projectile (see VengefulSoul below)
    public static bool TryAbsorbAt(Vector2 pos)
    {
        foreach (ProjectileAbsorber a in all)
        {
            if (!a.IsActive || !a.InsideHorizon(pos)) continue;
            a.OnAbsorbed(pos);
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, horizonRadius);
        Gizmos.color = new Color(1f, 0f, 1f, 0.35f);
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}