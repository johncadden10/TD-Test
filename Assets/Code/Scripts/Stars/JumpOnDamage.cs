using UnityEngine;

public class JumpOnDamage : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float jumpDistance = 1.5f;   // world units
    [SerializeField] private float cooldown = 0.25f;      // stops rapid hits from chaining jumps
    [SerializeField] private float minDamage = 0f;        // ignore hits weaker than this

    private Health health;
    private EnemyMovement movement;
    private Rigidbody2D rb;
    private float nextJumpTime;

    private void Awake()
    {
        health = GetComponent<Health>();
        movement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()  { health.OnDamaged += HandleDamaged; }
    private void OnDisable() { health.OnDamaged -= HandleDamaged; }

    private void HandleDamaged(DamageInfo info)
    {
        if (Time.time < nextJumpTime) return;
        if (info.amount < minDamage) return;
        nextJumpTime = Time.time + cooldown;

        Jump();
    }

    private void Jump()
    {
        Transform[] path = LevelManager.main.path;
        Vector2 pos = rb != null ? rb.position : (Vector2)transform.position;
        float remaining = jumpDistance;

        // walk along the path segments, so the jump follows corners
        int index = movement.PathIndex;
        while (remaining > 0f && index < path.Length)
        {
            Vector2 waypoint = path[index].position;
            float dist = Vector2.Distance(pos, waypoint);

            if (dist > remaining)
            {
                pos = Vector2.MoveTowards(pos, waypoint, remaining);
                remaining = 0f;
            }
            else
            {
                pos = waypoint;
                remaining -= dist;
                index++;
            }
        }

        // stop one waypoint short of the end so EndPoint's trigger still handles the leak
        index = Mathf.Min(index, path.Length - 1);

        movement.SetPathProgress(index);
        if (rb != null) rb.position = pos;
        transform.position = pos;
    }
}