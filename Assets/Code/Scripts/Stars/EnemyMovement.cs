using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform target;
    private int pathIndex = 0;
    private StatusController status;

    public int PathIndex => pathIndex;

    private void Awake()
    {
        status = GetComponent<StatusController>();
    }

    private void Start()
    {
        target = LevelManager.main.path[pathIndex];
    }

    private void Update()
    {
        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;

            if (pathIndex >= LevelManager.main.path.Length)
            {
                // reaching the end is handled by EndPoint, so nothing here
                // (avoids double-counting enemiesAlive and skipping lives)
                return;
            }

            target = LevelManager.main.path[pathIndex];
        }
    }

    private void FixedUpdate()
    {
        float mult = status != null ? status.SpeedMultiplier : 1f;
        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed * mult;
    }

    public void SetPathProgress(int index)
    {
        pathIndex = Mathf.Clamp(index, 0, LevelManager.main.path.Length - 1);
        target = LevelManager.main.path[pathIndex];
    }

    public void AdvanceAlongPath(int waypoints)
    {
        int path = LevelManager.main.path.Length;
        int newIndex = Mathf.Min(pathIndex + waypoints, path - 1);
        if (newIndex <= pathIndex) return;

        pathIndex = newIndex;
        target = LevelManager.main.path[pathIndex];
    }

    public float DistanceRemaining
    {
        get
        {
            Transform[] path = LevelManager.main.path;
            if (target == null) return float.MaxValue;

            float d = Vector2.Distance(transform.position, target.position);
            for (int i = pathIndex; i < path.Length - 1; i++)
                d += Vector2.Distance(path[i].position, path[i + 1].position);
            return d;
        }
    }
}