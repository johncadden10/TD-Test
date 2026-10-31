using UnityEngine;

public class StarBolt : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float hitDistance = 0.2f;
    [SerializeField] private float maxLife = 5f; // safety net

    private Ghost target;
    private float lifetimeDamage;

    public void Init(Ghost t, float damage)
    {
        target = t;
        lifetimeDamage = damage;
    }

    private void Update()
    {
        maxLife -= Time.deltaTime;

        // target expired or was destroyed mid-flight
        if (target == null || !target.IsAlive || maxLife <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 pos = target.transform.position;
        Vector2 dir = (pos - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, pos, speed * Time.deltaTime);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (Vector2.Distance(transform.position, pos) <= hitDistance)
        {
            target.ReduceLifetime(lifetimeDamage);
            Destroy(gameObject);
        }
    }
}