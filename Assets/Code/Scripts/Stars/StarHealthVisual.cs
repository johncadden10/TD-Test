using UnityEngine;

public class StarHealthVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Health at which each tier ends")]
    [Tooltip("Remaining HP at or below this shows the next color. Strongest tier first.")]
    [SerializeField] private float[] tierThresholds = { 30f, 15f };  // blue above 30, yellow 15-30, red 15 or less

    private static readonly int Health01 = Animator.StringToHash("Health01");

    private Health health;
    private int lastTier = -1;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()  { health.OnDamaged += Refresh; }
    private void OnDisable() { health.OnDamaged -= Refresh; }

    private void Start() { Refresh(default); }

    private void Refresh(DamageInfo _)
    {
        int tier = 0;
        foreach (float t in tierThresholds)
            if (health.HitPoints <= t) tier++;

        if (tier == lastTier) return;
        lastTier = tier;

        // tier 0 = first frame, last tier = final frame
        int frames = tierThresholds.Length; // number of steps after the first
        animator.SetFloat(Health01, frames == 0 ? 0f : (float)tier / frames);
    }
}