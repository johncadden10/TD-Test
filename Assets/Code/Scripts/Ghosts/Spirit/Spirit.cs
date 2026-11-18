using System.Collections.Generic;
using UnityEngine;

public class Spirit : Ghost
{
    [Header("Beam Visual")]
    [SerializeField] private Material beamMaterial;
    [SerializeField] private Color beamColor = new Color(0.5f, 0.9f, 1f, 0.95f);
    [SerializeField] private float beamWidth = 0.1f;
    [SerializeField] private float beamShowTime = 0.15f;

    [Header("Chill")]
    [SerializeField] private float chillMultiplier = 0.5f;
    [SerializeField] private float chillDuration = 3f;

    [Header("Frost Debuffs (1-3)")]
    [SerializeField] private float vulnerableMultiplier = 1.3f;
    [SerializeField] private float weakenMultiplier = 0.7f;
    [SerializeField] private float debuffDuration = 4f;

    [Header("Cold Snap (2-2)")]
    [SerializeField] private float snapDuration = 3f;
    [SerializeField] private float snapCooldown = 15f;
    [SerializeField] private GameObject snapEffectPrefab; // optional

    // set by upgrade modifiers
    public bool FreezeOnHit { get; set; }
    public bool PermanentChill { get; set; }
    public bool DebuffsChilled { get; set; }
    public bool HitsAll { get; set; }
    public bool ColdSnap { get; set; }

    private float nextSnapAllowed;
    private Material runtimeBeamMaterial;

    private void OnEnable()  { OnLifetimeReduced += HandleDamaged; }
    private void OnDisable() { OnLifetimeReduced -= HandleDamaged; }

    // ---- the beam attack (called by the OnAttackFire animation event) ----
    protected override void Shoot()
    {
        Vector2 origin = firingPoint != null ? (Vector2)firingPoint.position : (Vector2)transform.position;

        if (HitsAll)
        {
            foreach (Health h in StarsWithin(TargetingRange)) Strike(h, origin);
        }
        else if (target != null)
        {
            Health h = target.GetComponentInParent<Health>();
            if (h != null) Strike(h, origin);
        }
    }

    private void Strike(Health h, Vector2 origin)
    {
        if (!h.IsTargetable) return;

        ShowBeam(origin, h.transform.position);

        // immune to cold or magic (or blocked, or under a Fusion threshold): the hit doesn't land, so nothing else happens
        bool landed = h.TakeDamage(MakeDamage(attackDamage));
        if (!landed) return;

        StatusController sc = h.GetComponent<StatusController>();
        if (sc == null) return;

        sc.AddSlow(chillMultiplier, chillDuration, true);
        if (!sc.IsChilled) return; // cold-slow immune, so no follow-up effects

        if (FreezeOnHit) sc.Stun(chillDuration);                 // 1-1
        if (PermanentChill) sc.AddPermanentChill(chillMultiplier); // 1-2

        if (DebuffsChilled)                                      // 1-3
        {
            sc.AddVulnerable(vulnerableMultiplier, debuffDuration);
            sc.AddWeaken(weakenMultiplier, debuffDuration);
        }
    }

    // ---- 2-2: when the Spirit is hurt, freeze everything on screen ----
    private void HandleDamaged(float seconds)
    {
        if (!ColdSnap || Time.time < nextSnapAllowed) return;
        nextSnapAllowed = Time.time + snapCooldown;

        Camera cam = Camera.main;

        foreach (Health h in FindObjectsByType<Health>(FindObjectsSortMode.None))
        {
            if (!h.IsTargetable) continue;

            ResistanceModifier r = h.GetComponent<ResistanceModifier>();
            if (r != null && r.IsImmuneTo(DamageType)) continue;

            if (cam != null)
            {
                Vector3 v = cam.WorldToViewportPoint(h.transform.position);
                if (v.x < 0f || v.x > 1f || v.y < 0f || v.y > 1f) continue;
            }

            StatusController sc = h.GetComponent<StatusController>();
            if (sc == null) continue;

            sc.AddSlow(chillMultiplier, snapDuration, true);
            if (sc.IsChilled) sc.Stun(snapDuration);
        }

        if (snapEffectPrefab != null)
            Instantiate(snapEffectPrefab, transform.position, Quaternion.identity);
    }

    // ---- helpers ----
    private List<Health> StarsWithin(float radius)
    {
        var result = new List<Health>();
        var seen = new HashSet<Health>();

        foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, radius, enemyMask))
        {
            Health h = c.GetComponentInParent<Health>();
            if (h != null && seen.Add(h)) result.Add(h);
        }
        return result;
    }

    private void ShowBeam(Vector2 from, Vector2 to)
    {
        if (runtimeBeamMaterial == null)
            runtimeBeamMaterial = beamMaterial != null ? beamMaterial : new Material(Shader.Find("Sprites/Default"));

        GameObject go = new GameObject("SpiritBeam");
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.useWorldSpace = true;
        lr.SetPosition(0, from);
        lr.SetPosition(1, to);
        lr.material = runtimeBeamMaterial;
        lr.startColor = beamColor;
        lr.endColor = beamColor;
        lr.startWidth = beamWidth;
        lr.endWidth = beamWidth;

        SpriteRenderer main = GetComponentInChildren<SpriteRenderer>();
        if (main != null)
        {
            lr.sortingLayerID = main.sortingLayerID;
            lr.sortingOrder = main.sortingOrder + 1;
        }

        Destroy(go, beamShowTime);
    }
}