using UnityEngine;

public class Banshee : Ghost
{
    [Header("Banshee References")]
    [SerializeField] private BansheeAttack bansheeAttack;

    protected override void Update()
    {
        timeUntilFire += Time.deltaTime;

        bool enemiesInRange = EnemiesInRange();

        if (towerAnimator != null)
            towerAnimator.SetAttackingBool(enemiesInRange);
        
        if (bansheeAttack != null)
            bansheeAttack.SetActive(enemiesInRange);

        if (timeUntilFire >= attackSpeed && enemiesInRange)
        {
            Shoot();
            timeUntilFire = 0f;
        }
    }

    private bool EnemiesInRange()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);
        return hits.Length > 0;
    }

    protected override void Shoot()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        foreach (RaycastHit2D hit in hits)
        {
            hit.transform.GetComponent<Health>().TakeDamage(attackDamage);
        }
    }
}