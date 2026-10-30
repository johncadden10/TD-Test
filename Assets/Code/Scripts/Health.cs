using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int hitPoints = 15;
    [SerializeField] private int livesToSubtract = 1;
    [SerializeField] private int ectoWorth = 0;

    private bool isDestroyed = false;

    public int LivesWorth => livesToSubtract;
    public int HitPoints => hitPoints;

    public void TakeDamage(int dmg)
    {
        hitPoints -= dmg;

        if (hitPoints <= 0 && !isDestroyed)
        {
            EnemySpawner.onEnemyDestroy.Invoke();
            LevelManager.main.IncreaseEcto(ectoWorth);
            isDestroyed = true;
            Destroy(gameObject);
        }
    }
}
