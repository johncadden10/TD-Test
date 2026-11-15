using UnityEngine;

public class LevelLayout : MonoBehaviour
{
    [SerializeField] private Transform pathRoot;       // children = waypoints, in order
    [SerializeField] private float cameraSize = 5f;    // orthographic size for this level

    public float CameraSize => cameraSize;

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