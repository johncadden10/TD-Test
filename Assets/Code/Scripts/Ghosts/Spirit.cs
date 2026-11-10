using UnityEngine;

public class Spirit : Ghost
{
    [Header("Spirit References")]
    [SerializeField] private SpiritAttack spiritEffect;

    [Header("Spirit Attributes")]
    [SerializeField] private float freezeTime = 2f;
    [SerializeField] private float slowPercent = 0.6f;

    private bool isAnimationPlaying = false;

    protected override void Update()
    {
        if (IsStunned) return;
        if (isAnimationPlaying) return;

        timeUntilFire += HasteDelta;

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

    // animation event at the frame the slow should apply
    public void OnSlowApply()
    {
        Shoot();
        if (spiritEffect != null)
            spiritEffect.TriggerEffect();
    }

    // animation event on the last frame
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
            StatusController status = hit.transform.GetComponent<StatusController>();
            if (status != null)
                status.AddSlow(slowPercent, freezeTime, DamageType.Has(DamageType.Cold));
        }
    }
}