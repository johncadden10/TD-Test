using UnityEngine;

public class PoltergeistBullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 6f;
    [SerializeField] private float bulletLifetime = 2.5f;
    [SerializeField] private float bulletDamage = 4f;
    
    private Transform target;
    //private float timeLeft;

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    private void Start()
    {
        //timeLeft = bulletLifetime;
        Destroy(gameObject, bulletLifetime);
    }

    private void Update()
    {
        //timeLeft -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        other.gameObject.GetComponent<Health>().TakeDamage(bulletDamage);
        Destroy(gameObject);
    }

    public void SetDirection(Vector2 direction)
    {
        rb.linearVelocity = direction * bulletSpeed;
    }
}
