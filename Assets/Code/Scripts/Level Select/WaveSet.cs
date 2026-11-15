using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tower Defense/Wave Set")]
public class WaveSet : ScriptableObject
{
    public List<Wave> waves;
}