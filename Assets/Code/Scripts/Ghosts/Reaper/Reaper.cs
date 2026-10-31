using System.Collections.Generic;
using UnityEngine;

public class Reaper : Ghost
{
    private static readonly List<Reaper> all = new List<Reaper>();
    public static IReadOnlyList<Reaper> All => all;

    [Header("Reaper References")]
    [SerializeField] private ReaperText reaperEffect;
    [SerializeField] private Soul soulPrefab;
    [SerializeField] private GameObject vengefulSoulPrefab;

    [Header("Reaper Attributes")]
    [SerializeField] private int currencyGain = 20;
    [SerializeField] private float generationSpeed = 7.5f;
    [SerializeField] private int vengefulBaseDamage = 40;

    // set by modifiers
    public float SpeedMultiplier { get; set; } = 1f;
    public float AdjacencyMultiplier { get; set; } = 1f;
    public float ValueMultiplier { get; set; } = 1f;
    public float ValueGrowthPerSecond { get; set; } = 0f;
    public int MaxValueBonus { get; set; } = 0;
    public bool AutoCollect { get; set; } = false;
    [System.NonSerialized] public VengefulSettings Vengeful = new VengefulSettings();

    private float timeRemaining;
    private float spawnTime;
    private float progress;
    private bool isGenerating;

    private void OnEnable() { all.Add(this); }
    private void OnDisable() { all.Remove(this); }

    protected override void Start()
    {
        base.Start();
        spawnTime = Time.time;
        progress = 0f;
        Vengeful.prefab = vengefulSoulPrefab;
        Vengeful.baseDamage = vengefulBaseDamage;
        Vengeful.enemyMask = enemyMask;
        timeRemaining = Interval();
    }

    private float Interval() => generationSpeed / (SpeedMultiplier * AdjacencyMultiplier);

    protected override void Update()
    {
        if (IsStunned) return;
        if (isGenerating) return;

        progress += Time.deltaTime * SpeedMultiplier * AdjacencyMultiplier;

        if (progress >= generationSpeed)
        {
            progress = 0f;
            if (towerAnimator != null)
            {
                isGenerating = true;
                towerAnimator.SetGenerating();
            }
            else OnGenerateEcto();
        }
    }

    // animation event
    public void OnGenerateEcto()
    {
        int value = CurrentValue();

        if (AutoCollect)
        {
            LevelManager.main.IncreaseEcto(value);
        }
        else
        {
            Vector2 offset = Random.insideUnitCircle * 0.5f;
            Soul soul = Instantiate(soulPrefab, (Vector2)transform.position + offset, Quaternion.identity);
            soul.Init(value, Vengeful);
        }

        if (reaperEffect != null) reaperEffect.TriggerEffect();
    }

    public void OnGenerateEnd()
    {
        isGenerating = false;
    }

    private int CurrentValue()
    {
        float bonus = Mathf.Min((Time.time - spawnTime) * ValueGrowthPerSecond, MaxValueBonus);
        return Mathf.RoundToInt((currencyGain + bonus) * ValueMultiplier);
    }

    protected override void Shoot() { }
}