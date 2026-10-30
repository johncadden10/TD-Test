using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelDefinition
{
    public string levelId;
    public string displayName;
    public string sceneName;
    public int essenceReward;
    public int worldIndex;          // which world this level belongs to
    public Vector2 mapPosition;     // position on the constellation map
    public List<int> connections;   // indices of levels this connects to visually
    public Sprite preview;
    public List<string> enemyTypes; // shown on loadout screen before entering
    public int towerSlotCount = 4;
}

[System.Serializable]
public class WorldDefinition
{
    public string worldName;
    public Sprite background;
    public Color constellationColor = Color.white;
    public List<LevelDefinition> levels;
}