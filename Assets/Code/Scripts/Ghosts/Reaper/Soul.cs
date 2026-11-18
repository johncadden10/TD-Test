using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    private static readonly List<Soul> all = new List<Soul>();
    public static IReadOnlyList<Soul> All => all;

    [Header("Effects")]
    [SerializeField] private ParticleSystem[] shrinkingEffects;  // scaled along with the soul
    [SerializeField] private bool stopEmissionOnArrive = true;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 6f;

    [Header("Spawn Animation")]
    [SerializeField] private float growTime = 0.25f;
    [SerializeField] private float hopVelocity = 3.5f;
    [SerializeField] private Vector2 horizontalSpeedRange = new Vector2(0.4f, 1.2f);
    [SerializeField] private float gravity = 12f;

    [Header("Magnet")]
    [SerializeField] private float magnetAcceleration = 2f;
    [SerializeField] private float arriveDistance = 0.1f;

    [Header("Collect Flight")]
    [SerializeField] private float flyTime = 0.45f;     // travel to the counter
    [SerializeField] private float shrinkTime = 0.25f;  // shrink once stopped on the counter

    private int value;
    private VengefulSettings vengeful;
    private bool finished;

    private Vector2 ground;
    private float z;
    private float height, vy, vx;
    private bool landed;
    private float age, pullTime;
    private Vector3 baseScale;
    private Object magnetOwner;

    // collect flight
    private bool flying;
    private bool arrived;
    private float flyT, shrinkT;
    private Vector3 flyStart, flyStartScale;

    public bool Landed => landed;

    private void Awake()
    {
        ground = transform.position;
        z = transform.position.z;

        baseScale = transform.localScale;
        transform.localScale = Vector3.zero;

        vy = hopVelocity;
        float dir = Random.value < 0.5f ? -1f : 1f;
        vx = dir * Random.Range(horizontalSpeedRange.x, horizontalSpeedRange.y);
    }

    private void OnEnable()  { all.Add(this); }
    private void OnDisable() { all.Remove(this); }

    public void Init(int soulValue, VengefulSettings settings)
    {
        value = soulValue;
        vengeful = settings;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        if (flying) { UpdateFlight(dt); return; }

        age += dt;

        float t = growTime <= 0f ? 1f : Mathf.Clamp01(age / growTime);
        float eased = 1f - (1f - t) * (1f - t);
        transform.localScale = baseScale * eased;

        if (!landed) UpdateHop(dt);

        lifetime -= dt;
        if (lifetime <= 0f) Expire();
    }

    private void UpdateHop(float dt)
    {
        vy -= gravity * dt;
        height += vy * dt;
        ground.x += vx * dt;

        if (height <= 0f)
        {
            height = 0f;
            landed = true;
        }

        transform.position = new Vector3(ground.x, ground.y + height, z);
    }

    // ---- magnet ----
    public bool PullToward(Object owner, Vector2 center, float baseSpeed, float dt)
    {
        if (!landed || finished) return false;

        if (magnetOwner != null && magnetOwner != owner)
        {
            Component current = magnetOwner as Component;
            Reaper r = current as Reaper;
            if (r != null && r.IsStunned) magnetOwner = null; // a stunned owner gives the soul up
            else if (current != null)
            {
                float currentDist = Vector2.Distance(ground, current.transform.position);
                float newDist = Vector2.Distance(ground, center);
                if (newDist >= currentDist - 0.25f) return false;
            }
        }
        magnetOwner = owner;

        pullTime += dt;
        float speed = baseSpeed * (1f + pullTime * magnetAcceleration);
        ground = Vector2.MoveTowards(ground, center, speed * dt);
        transform.position = new Vector3(ground.x, ground.y, z);

        if (Vector2.Distance(ground, center) <= arriveDistance)
        {
            Collect();
            return true;
        }
        return false;
    }

    public void ReleaseMagnet(Object owner)
    {
        if (magnetOwner == owner) magnetOwner = null;
    }

    // ---- collecting ----
    public void Collect()
    {
        if (finished) return;
        finished = true;

        all.Remove(this); // no more clicks, magnets or Reaper pulls

        foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        // no counter in the scene (or no flight wanted): pay out and vanish
        if (EctoTarget.main == null || flyTime <= 0f)
        {
            LevelManager.main.IncreaseEcto(value);
            Destroy(gameObject);
            return;
        }

        flying = true;
        arrived = false;
        flyT = 0f;
        shrinkT = 0f;
        flyStart = transform.position;
        flyStartScale = transform.localScale;
        //extraStartScales = new Vector3[extraScaledObjects.Length];
        //for (int i = 0; i < extraScaledObjects.Length; i++)
        //    if (extraScaledObjects[i] != null) extraStartScales[i] = extraScaledObjects[i].localScale;
    }

    private void UpdateFlight(float dt)
    {
        if (EctoTarget.main == null)
        {
            LevelManager.main.IncreaseEcto(value);
            Destroy(gameObject);
            return;
        }

        // phase 2: stopped on the counter, shrinking away
        if (arrived)
        {
            shrinkT += dt / Mathf.Max(0.0001f, shrinkTime);
            float s = Mathf.Clamp01(shrinkT);
            transform.position = new Vector3(EctoTarget.main.WorldPoint().x, EctoTarget.main.WorldPoint().y, z);
            transform.localScale = Vector3.Lerp(flyStartScale, Vector3.zero, Mathf.SmoothStep(0f, 1f, s));

            if (s >= 1f) Destroy(gameObject);
            return;
        }

        // phase 1: speeds up, then slows to a stop on the target
        flyT += dt / flyTime;
        float t = Mathf.Clamp01(flyT);
        float eased = t * t * (3f - 2f * t); // smoothstep: ease in and out

        Vector3 dest = EctoTarget.main.WorldPoint();
        Vector3 p = Vector3.Lerp(flyStart, dest, eased);
        transform.position = new Vector3(p.x, p.y, z);

        if (t >= 1f) Arrive();
    }

    private void Arrive()
    {
        arrived = true;
        foreach (ParticleSystem ps in shrinkingEffects)
        {
            if (ps == null) continue;
            if (stopEmissionOnArrive) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        // paid out the moment it stops, not after the shrink
        LevelManager.main.IncreaseEcto(value);
        if (EctoTarget.main != null) EctoTarget.main.Pulse();
    }

    private void Expire()
    {
        if (finished) return;
        finished = true;

        if (vengeful != null && vengeful.enabled && vengeful.prefab != null)
        {
            GameObject obj = Instantiate(vengeful.prefab, transform.position, Quaternion.identity);
            obj.GetComponent<VengefulSoul>().Init(vengeful);
        }
        Destroy(gameObject);
    }
}