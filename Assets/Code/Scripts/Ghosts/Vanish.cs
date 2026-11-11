using UnityEngine;

public class Vanish : Ghost
{
    [Header("Vanish References")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Vanish Attributes")]
    [SerializeField] private float multiTurnRate = 240f; // used only when firing several clouds, so they fan out then curve back

    // changed by upgrade modifiers
    public int ShotCount { get; set; } = 1;
    public float SpreadAngle { get; set; } = 0f;
    public int PierceBonus { get; set; } = 0;
    public float CloudSpeedMultiplier { get; set; } = 1f;
    public bool CloudsIgnoreBelted { get; set; }
    public bool CloudsIgnoreAbsorbers { get; set; }
    public float BlastDelay { get; set; } = 0f;       // 0 = no blast
    public float BlastRadius { get; set; } = 0f;
    public float BlastDamageMultiplier { get; set; } = 1f;
    public bool CloudsReseek { get; set; }
    public float TurnRateMultiplier { get; set; } = 1f;

    protected override void Shoot()
    {
        Vector2 baseDir = ((Vector2)target.position - (Vector2)firingPoint.position).normalized;
        float step = ShotCount > 1 ? SpreadAngle / (ShotCount - 1) : 0f;
        float start = -SpreadAngle / 2f;
        float turn = (ShotCount > 1 || CloudsReseek) ? multiTurnRate * TurnRateMultiplier : 0f; // a single cloud tracks perfectly, as before

        for (int i = 0; i < ShotCount; i++)
        {
            Vector2 dir = Rotate(baseDir, start + step * i);

            GameObject obj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
            Projectile p = obj.GetComponent<Projectile>();

            p.SetDamageType(DamageType);
            p.Pierce += PierceBonus;
            p.SpeedMultiplier = CloudSpeedMultiplier;
            p.IgnoreBelted = CloudsIgnoreBelted;
            p.IgnoreAbsorbers = CloudsIgnoreAbsorbers;
            p.Reseek = CloudsReseek;
            if (BlastDelay > 0f) p.ConfigureBlast(BlastDelay, BlastRadius, BlastDamageMultiplier);

            p.Launch(dir, target, turn);
        }
    }

    private static Vector2 Rotate(Vector2 v, float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }
}