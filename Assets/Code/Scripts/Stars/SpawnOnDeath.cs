using UnityEngine;

public class SpawnOnDeath : MonoBehaviour, IDeathEffect
{
    [System.Serializable]
    public struct Drop
    {
        public GameObject prefab;
        public int count;
    }

    [SerializeField] private Drop[] drops;
    [SerializeField] private float spread = 0.4f; // random offset so they don't stack

    private EnemyMovement movement;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
    }

    public void OnDeath(DamageInfo killingBlow)
    {
        foreach (Drop drop in drops)
        {
            for (int i = 0; i < drop.count; i++)
            {
                Vector2 offset = Random.insideUnitCircle * spread;
                GameObject obj = Instantiate(drop.prefab, (Vector2)transform.position + offset, Quaternion.identity);

                EnemyMovement m = obj.GetComponent<EnemyMovement>();
                if (m != null && movement != null)
                    m.SetPathProgress(movement.PathIndex);

                EnemySpawner.RegisterSpawnedEnemy();
            }
        }
    }
}