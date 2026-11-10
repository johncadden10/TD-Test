using System.Collections.Generic;
using UnityEngine;

public class FlameZone : MonoBehaviour
{
    private float radius, damage, tickInterval, endTime, nextTick;
    private LayerMask mask;
    private bool stuns;

    public void Init(float radius, float duration, float damagePerTick, float tickInterval, LayerMask mask, bool stuns)
    {
        this.radius = radius;
        this.damage = damagePerTick;
        this.tickInterval = tickInterval;
        this.mask = mask;
        this.stuns = stuns;

        endTime = Time.time + duration;
        nextTick = Time.time;
        transform.localScale = Vector3.one * radius * 2f; // assumes a 1-unit sprite
    }

    private void Update()
    {
        if (Time.time >= endTime) { Destroy(gameObject); return; }

        if (Time.time >= nextTick)
        {
            nextTick += tickInterval;
            Tick();
        }
    }

    private void Tick()
    {
        var seen = new HashSet<Health>();

        foreach (Collider2D c in Physics2D.OverlapCircleAll(transform.position, radius, mask))
        {
            Health h = c.GetComponentInParent<Health>();
            if (h == null || !seen.Add(h)) continue;

            StatusController status = h.GetComponent<StatusController>();
            bool landed = h.TakeDamage(new DamageInfo(damage, DamageType.Fire));

            // stunned until the flames disappear, but only if the damage actually landed
            if (landed && stuns && status != null)
                status.Stun(endTime - Time.time);
        }
    }
}