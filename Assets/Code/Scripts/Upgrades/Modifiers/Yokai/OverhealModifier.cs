using UnityEngine;

// Path 2-2: restored lifetime can exceed the maximum by up to 15 seconds
public class OverhealModifier : YokaiModifier
{
    [SerializeField] private float seconds = 15f;
    protected override void Apply() { yokai.OverhealSeconds = Mathf.Max(yokai.OverhealSeconds, seconds); }
}