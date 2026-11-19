using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CustomEditor(typeof(LevelLayout))]
public class LevelLayoutEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LevelLayout layout = (LevelLayout)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Bake Plots From Tilemap"))
            BakePlots(layout);

        if (GUILayout.Button("Snap Waypoints To Cell Centers"))
            SnapPath(layout);
    }

    private static void BakePlots(LevelLayout layout)
    {
        if (layout.plotMarkers == null || layout.plotPrefab == null || layout.plotsRoot == null)
        {
            Debug.LogError("Assign plotMarkers, plotPrefab and plotsRoot first.");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(layout.plotsRoot.gameObject, "Bake Plots");

        // clear the previous bake
        for (int i = layout.plotsRoot.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(layout.plotsRoot.GetChild(i).gameObject);

        Tilemap map = layout.plotMarkers;
        map.CompressBounds();
        int count = 0;

        foreach (Vector3Int cell in map.cellBounds.allPositionsWithin)
        {
            if (!map.HasTile(cell)) continue;

            GameObject plot = (GameObject)PrefabUtility.InstantiatePrefab(layout.plotPrefab, layout.plotsRoot);
            plot.transform.position = map.GetCellCenterWorld(cell);
            plot.name = $"Plot_{cell.x}_{cell.y}";
            Undo.RegisterCreatedObjectUndo(plot, "Bake Plots");
            count++;
        }

        EditorUtility.SetDirty(layout);
        Debug.Log($"Baked {count} plots.");
    }

    private static void SnapPath(LevelLayout layout)
    {
        Grid grid = layout.GetComponentInChildren<Grid>();
        if (grid == null) return;

        foreach (Transform wp in layout.GetPath())
        {
            Undo.RecordObject(wp, "Snap Waypoint");
            Vector3Int cell = grid.WorldToCell(wp.position);
            wp.position = grid.GetCellCenterWorld(cell);
        }
    }
}