using UnityEngine;

// Path 2-1: harvests straight into your Ecto, and draws in other souls on the field
public class AutoHarvestModifier : ReaperModifier
{
    [SerializeField] private float radius = 3f;
    [SerializeField] private float speed = 5f;

    protected override void Apply()
    {
        reaper.AutoCollect = true;
        reaper.MagnetRadius = Mathf.Max(reaper.MagnetRadius, radius);
        reaper.MagnetSpeed = Mathf.Max(reaper.MagnetSpeed, speed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}