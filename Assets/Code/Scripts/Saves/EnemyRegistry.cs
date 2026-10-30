using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyRegistry", menuName = "Tower Defense/Enemy Registry")]
public class EnemyRegistry : ScriptableObject
{
    public static EnemyRegistry main;

    public List<EnemyDefinition> enemies;

    private void OnEnable()
    {
        main = this;
    }

    public EnemyDefinition GetById(string id)
    {
        return enemies.Find(e => e.enemyId == id);
    }
}