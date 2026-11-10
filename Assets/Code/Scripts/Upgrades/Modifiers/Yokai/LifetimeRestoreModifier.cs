using UnityEngine;

// Path 2-1: ghosts in the blast regain 15 seconds
public class LifetimeRestoreModifier : YokaiModifier
{
    [SerializeField] private float seconds = 15f;
    protected override void Apply() { yokai.LifetimeRestoreSeconds = Mathf.Max(yokai.LifetimeRestoreSeconds, seconds); }
}