using UnityEngine;

// Path 1-1: flames linger after the blast
public class LingeringFlamesModifier : YokaiModifier
{
    [SerializeField] private float duration = 5f;
    protected override void Apply() { yokai.FlameDuration = Mathf.Max(yokai.FlameDuration, duration); }
}