using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float hitPoints = 15f;
    [SerializeField] private int livesToSubtract = 1;
    [SerializeField] private int currencyWorth = 0;

    private bool isDestroyed = false;
    private float maxHitPoints;

    public float HitPoints => hitPoints;
    public int LivesWorth => livesToSubtract;
    public float MaxHitPoints => maxHitPoints;
    public bool IsFull => hitPoints >= maxHitPoints;
    public event System.Action<DamageInfo> OnDamaged;

    public bool IsTargetable { get; set; } = true;
    

    private IDamageModifier[] modifiers;
    private IDeathEffect[] deathEffects;

    private void Awake()
    {
        maxHitPoints = hitPoints;
        RefreshModifiers();
        deathEffects = GetComponents<IDeathEffect>();
    }

    public void RefreshModifiers()
    {
        modifiers = GetComponents<IDamageModifier>();
        System.Array.Sort(modifiers, (a, b) => a.Order.CompareTo(b.Order));
    }

    public void Heal(float amount)
    {
        if (isDestroyed || amount <= 0f) return;
        hitPoints = Mathf.Min(hitPoints + amount, maxHitPoints);
    }

    // old signature, so existing ghosts still compile
    public void TakeDamage(float dmg) => TakeDamage(new DamageInfo(dmg));

    public bool TakeDamage(DamageInfo info)
    {
        if (isDestroyed || (!IsTargetable && !info.ignoreHidden)) return false;

        foreach (IDamageModifier m in modifiers) m.Modify(ref info);
        if (info.amount <= 0f) return false;

        hitPoints -= info.amount;
        OnDamaged?.Invoke(info);

        if (hitPoints <= 0f) Die(info);
        return true;
    }

    private void Die(DamageInfo killingBlow)
    {
        isDestroyed = true;
        foreach (IDeathEffect e in deathEffects) e.OnDeath(killingBlow);
        EnemySpawner.onEnemyDestroy.Invoke();
        LevelManager.main.IncreaseEcto(currencyWorth);
        Destroy(gameObject);
    }
}