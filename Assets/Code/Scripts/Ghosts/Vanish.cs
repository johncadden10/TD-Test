using UnityEngine;

public class Vanish : Ghost
{
    [Header("Vanish References")]
    [SerializeField] private GameObject bulletPrefab;

    protected override void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        VanishBullet bulletScript = bulletObj.GetComponent<VanishBullet>();
        bulletScript.SetTarget(target);
    }
}