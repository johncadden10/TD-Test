using System;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] private float baseDiameter = 1f;               // world-unit width of the sprite at scale 1
    [SerializeField] private bool impactFromAnimationEvent = true;
    [SerializeField] private float fallbackImpactDelay = 0f;        // used when the event is off
    [SerializeField] private float maxLife = 3f;                    // safety net, longer than the clip

    private Action onImpact;
    private bool impacted;

    public void Init(float radius, Action impact)
    {
        transform.localScale = Vector3.one * (radius * 2f / baseDiameter);
        onImpact = impact;

        if (!impactFromAnimationEvent) Invoke(nameof(OnImpact), fallbackImpactDelay);
        Destroy(gameObject, maxLife);
    }

    // animation event on the frame the blast should land
    public void OnImpact()
    {
        if (impacted) return;
        impacted = true;
        onImpact?.Invoke();
    }

    // animation event on the last frame
    public void OnEffectEnd()
    {
        OnImpact(); // fires the blast if the impact event was forgotten
        Destroy(gameObject);
    }
}