using UnityEngine;

public class Phantom : Ghost
{
    [Header("Phantom References")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Phantom Attributes")]
    [SerializeField] private float baseTurnRate = 270f; // degrees per second, 0 = perfect tracking

    [Header("Flames (2-3)")]
    [SerializeField] private FlameZone flameZonePrefab;
    [SerializeField] private float flameRadius = 1f;
    [SerializeField] private float flameDamagePerTick = 2f;
    [SerializeField] private float flameTickInterval = 0.5f;

    // changed by upgrade modifiers
    public int ShotCount { get; set; } = 1;
    public float SpreadAngle { get; set; } = 0f;
    public int PierceBonus { get; set; } = 0;
    public float OrbSpeedMultiplier { get; set; } = 1f;
    public float TurnRateMultiplier { get; set; } = 1f;
    public bool OrbsReseek { get; set; }
    public float OrbDamageGrowth { get; set; }
    public float OrbSizeGrowth { get; set; }
    public float OrbGrowthCap { get; set; } = 3f;
    public float OrbLifetimeMultiplier { get; set; } = 1f;
    public bool OrbsIgnoreAbsorbers { get; set; }
    public float FlameDuration { get; set; }   // 0 = no flames

    protected override void Shoot()
    {
        Vector2 baseDir = ((Vector2)target.position - (Vector2)firingPoint.position).normalized;
        float step = ShotCount > 1 ? SpreadAngle / (ShotCount - 1) : 0f;
        float start = -SpreadAngle / 2f;

        for (int i = 0; i < ShotCount; i++)
        {
            Vector2 dir = Rotate(baseDir, start + step * i);

            GameObject obj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
            Projectile p = obj.GetComponent<Projectile>();

            p.SetDamageType(DamageType);
            p.Reseek = OrbsReseek;
            p.Pierce += PierceBonus + TempPierce;
            p.SpeedMultiplier = OrbSpeedMultiplier;
            p.Launch(dir, target, baseTurnRate * TurnRateMultiplier);
            p.Lifetime *= OrbLifetimeMultiplier;
            p.GrowthDamagePerSecond = OrbDamageGrowth;
            p.GrowthSizePerSecond = OrbSizeGrowth;
            p.GrowthCap = OrbGrowthCap;
            p.IgnoreAbsorbers = OrbsIgnoreAbsorbers;
            if (FlameDuration > 0f && flameZonePrefab != null)
                p.ConfigureFlames(flameZonePrefab, flameRadius, FlameDuration, flameDamagePerTick, flameTickInterval, enemyMask);
        }
    }

    private static Vector2 Rotate(Vector2 v, float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }
}