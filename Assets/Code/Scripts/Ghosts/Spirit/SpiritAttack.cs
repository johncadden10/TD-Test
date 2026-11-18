using UnityEngine;

public class SpiritAttack : MonoBehaviour
{
    [SerializeField] private Animator effectAnimator;
    private static readonly int PlayEffect = Animator.StringToHash("PlayEffect");

    public void TriggerEffect()
    {
        gameObject.SetActive(true);
        effectAnimator.SetTrigger(PlayEffect);
    }

    public void OnEffectEnd()
    {
        gameObject.SetActive(false);
    }
}