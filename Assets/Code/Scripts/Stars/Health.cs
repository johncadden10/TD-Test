using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int hitPoints = 15;
    [SerializeField] private int livesToSubtract = 1;
    [SerializeField] private int currencyWorth = 0;

    private bool isDestroyed = false;

    public int HitPoints => hitPoints;
    public int LivesWorth => livesToSubtract;
    public event System.Action<DamageInfo> OnDamaged;

    private IDamageModifier[] modifiers;
    private IDeathEffect[] deathEffects;

    private void Awake()
    {
        modifiers = GetComponents<IDamageModifier>();
        System.Array.Sort(modifiers, (a, b) => a.Order.CompareTo(b.Order));
        deathEffects = GetComponents<IDeathEffect>();
    }

    // old signature, so existing ghosts still compile
    public void TakeDamage(int dmg) => TakeDamage(new DamageInfo(dmg));

    public void TakeDamage(DamageInfo info)
    {
        if (isDestroyed) return;

        foreach (IDamageModifier m in modifiers) m.Modify(ref info);
        if (info.amount <= 0) return;

        hitPoints -= info.amount;
        OnDamaged?.Invoke(info);

        if (hitPoints <= 0) Die(info);
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