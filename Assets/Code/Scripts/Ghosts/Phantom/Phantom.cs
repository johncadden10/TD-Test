using UnityEngine;

public class Phantom : Ghost
{
    [Header("Phantom References")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Phantom Attributes")]
    [SerializeField] private float baseTurnRate = 270f; // degrees per second, 0 = perfect tracking

    // changed by upgrade modifiers
    public int ShotCount { get; set; } = 1;
    public float SpreadAngle { get; set; } = 0f;
    public int PierceBonus { get; set; } = 0;
    public float OrbSpeedMultiplier { get; set; } = 1f;
    public float TurnRateMultiplier { get; set; } = 1f;

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
            p.Pierce += PierceBonus;
            p.SpeedMultiplier = OrbSpeedMultiplier;
            p.Launch(dir, target, baseTurnRate * TurnRateMultiplier);
        }
    }

    private static Vector2 Rotate(Vector2 v, float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }
}