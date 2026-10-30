using System.Collections.Generic;

public static class LevelLoadout
{
    public class Entry
    {
        public TowerDefinition tower;
        public int pathIndex; // 0 = first path, 1 = second path
    }

    public static List<Entry> entries = new List<Entry>();
}