using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    private class SpeedEffect { public float multiplier; public float timeLeft; public bool cold; }
    private readonly List<SpeedEffect> speedEffects = new List<SpeedEffect>();

    [SerializeField] private bool immuneToColdSlow;
    public float StunTimeLeft { get; private set; }
    public bool IsStunned => StunTimeLeft > 0f;

    public float SpeedMultiplier
    {
        get
        {
            float m = 1f;
            foreach (SpeedEffect e in speedEffects) m = Mathf.Min(m, e.multiplier); // strongest slow wins
            return IsStunned ? 0f : m;
        }
    }

    public void AddSlow(float multiplier, float duration, bool isCold = false)
    {
        if (isCold && immuneToColdSlow) return;
        speedEffects.Add(new SpeedEffect { multiplier = multiplier, timeLeft = duration, cold = isCold });
    }

    public void Stun(float duration) => StunTimeLeft = Mathf.Max(StunTimeLeft, duration);

    private void Update()
    {
        StunTimeLeft = Mathf.Max(0f, StunTimeLeft - Time.deltaTime);
        for (int i = speedEffects.Count - 1; i >= 0; i--)
        {
            speedEffects[i].timeLeft -= Time.deltaTime;
            if (speedEffects[i].timeLeft <= 0f) speedEffects.RemoveAt(i);
        }
    }
}