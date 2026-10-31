using UnityEngine;

public class GhostShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StarBolt boltPrefab;

    [Header("Attributes")]
    [SerializeField] private float range = 4f;
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float lifetimeDamage = 2f;

    private float timer;
    private StatusController status;

    private void Awake()
    {
        status = GetComponent<StatusController>();
    }

    private void Update()
    {
        if (status != null && status.IsStunned) return;

        timer += Time.deltaTime;
        if (timer < fireInterval) return;

        Ghost target = Ghost.Nearest(transform.position, range);
        if (target == null) return; // keep the timer full until a ghost is in range

        Fire(target);
        timer = 0f;
    }

    private void Fire(Ghost target)
    {
        if (boltPrefab == null)
        {
            target.ReduceLifetime(lifetimeDamage); // fallback: instant effect
            return;
        }

        StarBolt bolt = Instantiate(boltPrefab, transform.position, Quaternion.identity);
        bolt.Init(target, lifetimeDamage);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}