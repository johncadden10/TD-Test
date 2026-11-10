using UnityEngine;

// Path 2-3: 25 seconds restored, and overhealed ghosts fire faster
public class OverhealSurgeModifier : YokaiModifier
{
    [SerializeField] private float restoreSeconds = 25f;
    [SerializeField] private float haste = 1.4f;

    protected override void Apply()
    {
        yokai.LifetimeRestoreSeconds = Mathf.Max(yokai.LifetimeRestoreSeconds, restoreSeconds);
        yokai.OverhealHaste = Mathf.Max(yokai.OverhealHaste, haste);
    }
}