using UnityEngine;

public class PhantomBullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 3f;
    [SerializeField] private float bulletLifetime = 5f;
    [SerializeField] private int bulletDamage = 4;

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

    private void FixedUpdate()
    {
        if (!target) return;

        Vector2 direction = (target.position - transform.position).normalized; 

        rb.linearVelocity = direction * bulletSpeed;

        //if (timeLeft <= 0f)
        //{
        //    Destroy(gameObject);
        //}
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        other.gameObject.GetComponent<Health>().TakeDamage(bulletDamage);
        Destroy(gameObject);
    }
}
