using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelRegistry", menuName = "Tower Defense/Level Registry")]
public class LevelRegistry : ScriptableObject
{
    public List<WorldDefinition> worlds;

    public LevelDefinition GetLevelById(string levelId)
    {
        foreach (WorldDefinition world in worlds)
        {
            LevelDefinition level = world.levels.Find(l => l.levelId == levelId);
            if (level != null) return level;
        }
        return null;
    }

    public int GetGlobalLevelIndex(string levelId)
    {
        int globalIndex = 0;
        foreach (WorldDefinition world in worlds)
        {
            foreach (LevelDefinition level in world.levels)
            {
                if (level.levelId == levelId) return globalIndex;
                globalIndex++;
            }
        }
        return -1;
    }

    public bool IsWorldUnlocked(int worldIndex, SaveData save)
    {
        if (worldIndex == 0) return true;
        int firstLevelOfWorld = 0;
        for (int i = 0; i < worldIndex; i++)
            firstLevelOfWorld += worlds[i].levels.Count;
        return save.highestLevelUnlocked >= firstLevelOfWorld;
    }
}