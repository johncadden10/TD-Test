using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            int lives = other.GetComponent<Health>().LivesWorth;
            LivesManager.main.LoseLife(lives);
            EnemySpawner.onEnemyDestroy.Invoke();
            Destroy(other.gameObject);
        }
    }
}