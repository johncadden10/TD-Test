using UnityEngine;

public class Wisp : Ghost
{
    protected override void Update()
    {
        if (IsStunned) return;
        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= attackSpeed)
        {
            Shoot();
            timeUntilFire = 0f;
        }
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