using System.Collections;
using UnityEngine;

public class Spirit : Ghost
{
    [Header("Spirit References")]
    [SerializeField] private SpiritAttack spiritAttack;

    [Header("Spirit Attributes")]
    [SerializeField] private float freezeTime = 2f;
    [SerializeField] private float slowPercent = 0.6f;

    private bool isAnimationPlaying = false;

    protected override void Update()
    {
        if (isAnimationPlaying) return;

        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= attackSpeed)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

            if (hits.Length > 0)
            {
                if (towerAnimator != null)
                {
                    isAnimationPlaying = true;
                    towerAnimator.SetAttacking();
                }
                else
                {
                    Shoot();
                }
                timeUntilFire = 0f;
            }
        }
    }

    public void OnSlowApply()
    {
        Shoot();
        if (spiritAttack != null)
            spiritAttack.TriggerEffect(); // trigger effect when slow is applied
    }

    public void OnAttackEnd()
    {
        isAnimationPlaying = false;
        timeUntilFire = 0f;
    }

    protected override void Shoot()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

        foreach (RaycastHit2D hit in hits)
        {
            EnemyMovement em = hit.transform.GetComponent<EnemyMovement>();
            em.UpdateSpeed(slowPercent);
            StartCoroutine(ResetEnemySpeed(em));
        }
    }

    private IEnumerator ResetEnemySpeed(EnemyMovement em)
    {
        yield return new WaitForSeconds(freezeTime);
        em.ResetSpeed();
    }
}