using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerRegistry", menuName = "Tower Defense/Tower Registry")]
public class TowerRegistry : ScriptableObject
{
    public List<TowerDefinition> towers;

    public TowerDefinition GetById(string id)
    {
        return towers.Find(t => t.towerId == id);
    }
}