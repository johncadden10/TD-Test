using UnityEngine;

public class GhostDebuffer : MonoBehaviour
{
    public enum Mode { Pulse, Aura }

    [SerializeField] private Mode mode = Mode.Pulse;

    [Header("Shared")]
    [SerializeField] private float range = 4f;
    [SerializeField] private float coldDisableTime = 3f; // cold hits switch the attack off this long

    [Header("Pulse (Pulsar)")]
    [SerializeField] private float fireInterval = 3f;
    [SerializeField] private float pulseLifetimeDamage = 3f;
    [SerializeField] private float beamLength = 6f;
    [SerializeField] private float beamHitWidth = 0.3f;   // how close a ghost must be to the line
    [SerializeField] private float beamShowTime = 0.15f;
    [SerializeField] private Material beamMaterial;       // empty = default sprite material
    [SerializeField] private Color beamColor = new Color(0.4f, 1f, 0.5f, 0.9f);
    [SerializeField] private float beamVisualWidth = 0.12f;

    [Header("Aura (Magnetar)")]
    [SerializeField] private float drainPerSecond = 1f;   // lifetime seconds per second
    [SerializeField] private Transform auraVisual;        // optional, scaled to the radius

    private Health health;
    private StatusController status;
    private float timer;
    private float disabledUntil;

    private LineRenderer beam;
    private float beamHideAt;

    private bool CanAttack =>
        (status == null || !status.IsStunned) && Time.time >= disabledUntil;

    private void Awake()
    {
        health = GetComponent<Health>();
        status = GetComponent<StatusController>();

        if (auraVisual != null && mode == Mode.Aura)
            auraVisual.localScale = Vector3.one * range * 2f; // assumes a 1-unit sprite
    }

    private void OnEnable()  { health.OnDamaged += HandleDamaged; }
    private void OnDisable() { health.OnDamaged -= HandleDamaged; }

    private void HandleDamaged(DamageInfo info)
    {
        if (info.type.Has(DamageType.Cold))
            disabledUntil = Time.time + coldDisableTime;
    }

    private void Update()
    {
        if (beam != null && beam.enabled && Time.time >= beamHideAt)
            beam.enabled = false;

        bool active = CanAttack;
        if (auraVisual != null && mode == Mode.Aura)
            auraVisual.gameObject.SetActive(active);

        if (!active) return;

        if (mode == Mode.Pulse) UpdatePulse();
        else UpdateAura();
    }

    // ---- Aura ----
    private void UpdateAura()
    {
        float drain = drainPerSecond * Time.deltaTime;

        foreach (Ghost g in Ghost.Active)
        {
            if (!g.IsAlive) continue;
            if (Vector2.Distance(g.transform.position, transform.position) <= range)
                g.ReduceLifetime(drain);
        }
    }

    // ---- Pulse ----
    private void UpdatePulse()
    {
        timer += Time.deltaTime;
        if (timer < fireInterval) return;

        Ghost aim = Ghost.Nearest(transform.position, range);
        if (aim == null) return; // wait with a full timer until a ghost is in range

        timer = 0f;
        FirePulse(aim);
    }

    private void FirePulse(Ghost aim)
    {
        Vector2 start = transform.position;
        Vector2 dir = ((Vector2)aim.transform.position - start).normalized;
        Vector2 end = start + dir * beamLength;

        // pierces: every ghost along the line is hit
        foreach (Ghost g in Ghost.Active)
        {
            if (!g.IsAlive) continue;
            if (DistanceToSegment(g.transform.position, start, end) <= beamHitWidth)
                g.ReduceLifetime(pulseLifetimeDamage);
        }

        ShowBeam(start, end);
    }

    private static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude);
        return Vector2.Distance(p, a + ab * t);
    }

    private void ShowBeam(Vector2 start, Vector2 end)
    {
        if (beam == null) CreateBeam();

        beam.SetPosition(0, start);
        beam.SetPosition(1, end);
        beam.enabled = true;
        beamHideAt = Time.time + beamShowTime;
    }

    private void CreateBeam()
    {
        // on a child, so it can't clash with a ConstellationLink LineRenderer
        GameObject go = new GameObject("PulseBeam");
        go.transform.SetParent(transform, false);

        beam = go.AddComponent<LineRenderer>();
        beam.positionCount = 2;
        beam.useWorldSpace = true;
        beam.material = beamMaterial != null ? beamMaterial : new Material(Shader.Find("Sprites/Default"));
        beam.startColor = beamColor;
        beam.endColor = beamColor;
        beam.startWidth = beamVisualWidth;
        beam.endWidth = beamVisualWidth;

        SpriteRenderer main = GetComponentInChildren<SpriteRenderer>();
        if (main != null)
        {
            beam.sortingLayerID = main.sortingLayerID;
            beam.sortingOrder = main.sortingOrder + 1;
        }
        beam.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}