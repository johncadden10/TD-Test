using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLayout : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private Transform pathRoot;

    [Header("Tilemap Plots")]
    public Tilemap plotMarkers;
    public GameObject plotPrefab;
    public Transform plotsRoot;

    [Header("Camera")]
    [SerializeField] private float cameraSize = 5f;
    public float CameraSize => cameraSize;

    private void Awake()
    {
        // marker tiles are for editing only
        if (plotMarkers != null)
            plotMarkers.GetComponent<TilemapRenderer>().enabled = false;
    }

    public Transform[] GetPath()
    {
        Transform[] path = new Transform[pathRoot.childCount];
        for (int i = 0; i < path.Length; i++) path[i] = pathRoot.GetChild(i);
        return path;
    }

    private void OnDrawGizmos()
    {
        if (pathRoot == null) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < pathRoot.childCount; i++)
        {
            Gizmos.DrawSphere(pathRoot.GetChild(i).position, 0.12f);
            if (i > 0) Gizmos.DrawLine(pathRoot.GetChild(i - 1).position, pathRoot.GetChild(i).position);
        }
    }
}