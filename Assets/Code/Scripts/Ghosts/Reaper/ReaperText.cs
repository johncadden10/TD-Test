using UnityEngine;

public class ReaperText : MonoBehaviour
{
    [SerializeField] private Animator effectAnimator;
    private static readonly int PlayEffect = Animator.StringToHash("PlayEffect");

    public void TriggerEffect()
    {
        gameObject.SetActive(true);
        effectAnimator.SetTrigger(PlayEffect);
    }

    // animation event on last frame of effect clip
    public void OnEffectEnd()
    {
        gameObject.SetActive(false);
    }
}