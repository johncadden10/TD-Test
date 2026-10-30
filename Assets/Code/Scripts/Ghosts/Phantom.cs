using UnityEngine;

public class Phantom : Ghost
{
    [Header("Phantom References")]
    [SerializeField] private GameObject bulletPrefab;

    protected override void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        PhantomBullet bulletScript = bulletObj.GetComponent<PhantomBullet>();
        bulletScript.SetTarget(target);
    }
}