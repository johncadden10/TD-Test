using UnityEngine;

public class Poltergeist : Ghost
{
    [Header("Poltergeist References")]
    [SerializeField] private GameObject bulletPrefab;    // shards, also used for fragments and debris
    [SerializeField] private GameObject boulderPrefab;   // a Projectile prefab: slow, high bulletDamage

    [Header("Shotgun Settings")]
    [SerializeField] private int pelletCount = 5;
    [SerializeField] private float spreadAngle = 30f;

    [Header("Boulder")]
    [SerializeField] private float boulderTurnRate = 120f;
    [SerializeField] private int debrisPierce = 1;

    // changed by upgrade modifiers
    public int PierceBonus { get; set; }
    public float ShardSpeedMultiplier { get; set; } = 1f;
    public int SplitCount { get; set; }
    public float SplitDamageMultiplier { get; set; } = 0.5f;
    public float SplitLifetime { get; set; } = 0.8f;
    public float SpreadMultiplier { get; set; } = 1f;
    public bool BoulderMode { get; set; }
    public float BoulderStun { get; set; }
    public float BoulderStunRadius { get; set; }
    public float BoulderFusedBonus { get; set; } = 1f;

    protected override void Shoot()
    {
        if (BoulderMode && boulderPrefab != null) FireBoulder();
        else FireShards();
    }

    private void FireShards()
    {
        Vector2 baseDir = ((Vector2)target.position - (Vector2)firingPoint.position).normalized;
        float spread = spreadAngle * SpreadMultiplier;
        float step = pelletCount > 1 ? spread / (pelletCount - 1) : 0f;
        float start = -spread / 2f;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector2 dir = Rotate(baseDir, start + step * i);

            GameObject obj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
            Projectile p = obj.GetComponent<Projectile>();

            p.SetDamageType(DamageType);
            p.Pierce += PierceBonus + TempPierce;
            p.SpeedMultiplier = ShardSpeedMultiplier;
            if (SplitCount > 0) p.ConfigureSplit(bulletPrefab, SplitCount, SplitDamageMultiplier, SplitLifetime);

            p.SetDirection(dir); // straight shards, no homing
        }
    }

    private void FireBoulder()
    {
        Vector2 dir = ((Vector2)target.position - (Vector2)firingPoint.position).normalized;

        GameObject obj = Instantiate(boulderPrefab, firingPoint.position, Quaternion.identity);
        Projectile p = obj.GetComponent<Projectile>();

        p.SetDamageType(DamageType);
        p.StunOnHit = BoulderStun;
        p.StunRadius = BoulderStunRadius;
        p.FusedBonusMultiplier = BoulderFusedBonus;
        // no TempPierce here: the amplifier doesn't affect the boulder itself

        // captured now, because this ghost may be gone when the boulder lands
        GameObject shard = bulletPrefab;
        int count = pelletCount;
        float spread = spreadAngle * SpreadMultiplier;
        DamageType type = DamageType;
        int basePierce = debrisPierce;
        int pierceAtFire = TempPierce;

        p.OnBreak = (pos, heading) =>
        {
            // the amplifier may have started or ended during the flight, so read it again if we can
            int amp = this != null ? TempPierce : pierceAtFire;
            SpawnDebris(shard, pos, heading, count, spread, type, basePierce + amp);
        };

        p.Launch(dir, target, boulderTurnRate);
    }

    // the normal shard fan, thrown along the boulder's direction of travel
    private static void SpawnDebris(GameObject shard, Vector2 pos, Vector2 heading, int count, float spread, DamageType type, int pierce)
    {
        Vector2 baseDir = heading != Vector2.zero ? heading.normalized : Vector2.right;
        float step = count > 1 ? spread / (count - 1) : 0f;
        float start = -spread / 2f;

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(shard, pos, Quaternion.identity);
            Projectile p = obj.GetComponent<Projectile>();
            p.SetDamageType(type);
            p.Pierce += pierce;
            p.SetDirection(Rotate(baseDir, start + step * i));
        }
    }

    private static Vector2 Rotate(Vector2 v, float degrees)
    {
        float r = degrees * Mathf.Deg2Rad;
        float c = Mathf.Cos(r), s = Mathf.Sin(r);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }
}