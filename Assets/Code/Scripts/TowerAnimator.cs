using UnityEngine;

public class TowerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    private static readonly int IsAttacking = Animator.StringToHash("Attack");
    private static readonly int IsDying = Animator.StringToHash("isDying");
    private static readonly int IsGenerating = Animator.StringToHash("Harvest");
    private static readonly int IsAttackingBool = Animator.StringToHash("isAttacking");
    private static readonly int IsPreDeath = Animator.StringToHash("PreDeath");

    public void SetAttacking()
    {
        animator.SetTrigger(IsAttacking); // trigger instead of bool
    }

    public void SetDying()
    {
        animator.SetTrigger(IsDying);
    }

    public void SetGenerating()
    {
        animator.SetTrigger(IsGenerating);
    }

    public void SetAttackingBool(bool attacking)
    {
        animator.SetBool(IsAttackingBool, attacking);
    }

    public void SetPreDeath()
    {
        animator.SetTrigger(IsPreDeath);
    }
}