using UnityEngine;

public class Poltergeist : Ghost
{
    [Header("Poltergeist References")]
    [SerializeField] private GameObject bulletPrefab;

    [Header("Shotgun Settings")]
    [SerializeField] private int pelletCount = 5;
    [SerializeField] private float spreadAngle = 30f;

    protected override void Shoot()
    {
        Vector2 baseDirection = (target.position - firingPoint.position).normalized;
        float angleStep = pelletCount > 1 ? spreadAngle / (pelletCount - 1) : 0f;
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < pelletCount; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 pelletDirection = RotateVector(baseDirection, angle);

            GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
            PoltergeistBullet bulletScript = bulletObj.GetComponent<PoltergeistBullet>();
            bulletScript.Pierce += TempPierce;
            bulletScript.SetTarget(target);
            bulletScript.SetDirection(pelletDirection);
        }
    }

    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
    }
}