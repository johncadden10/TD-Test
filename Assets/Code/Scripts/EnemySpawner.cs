using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[System.Serializable]
public class EnemyGroup
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnInterval;
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public bool useCustomWave;
    public List<EnemyGroup> enemyGroups;
    public float timeBetweenGroups;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;

    [Header("Waves")]
    [SerializeField] private List<Wave> waves;

    [Header("Auto Scaling (fallback)")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float enemiesPerSecondCap = 8f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;

    [Header("Attributes")]
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("Victory")]
    [SerializeField] private string victorySceneName = "VictoryScene";
    [SerializeField] private float victoryDelay = 3f;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWave = 0;
    private int enemiesAlive;
    private bool isSpawning = false;
    private bool victoryTriggered = false;

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
        StartCoroutine(StartWave());
    }

    private void Update()
    {
        if (enemiesAlive == 0 && !isSpawning && !victoryTriggered)
        {
            EndWave();
        }
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    private IEnumerator StartWave()
    {
        isSpawning = true;
        yield return new WaitForSeconds(timeBetweenWaves);

        if (currentWave < waves.Count && waves[currentWave].useCustomWave)
        {
            yield return SpawnCustomWave(waves[currentWave]);
        }
        else
        {
            yield return SpawnAutoWave();
        }

        isSpawning = false;
    }

    private IEnumerator SpawnCustomWave(Wave wave)
    {
        Debug.Log($"Starting wave: {wave.waveName}");

        foreach (EnemyGroup group in wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyPrefab);
                yield return new WaitForSeconds(group.spawnInterval);
            }

            yield return new WaitForSeconds(wave.timeBetweenGroups);
        }
    }

    private IEnumerator SpawnAutoWave()
    {
        Debug.Log($"Starting auto wave {currentWave + 1}");

        int enemiesLeftToSpawn = EnemiesPerWave();
        float eps = EnemiesPerSecond();
        float timeSinceLastSpawn = 0f;

        while (enemiesLeftToSpawn > 0)
        {
            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= (1f / eps))
            {
                int index = Random.Range(0, enemyPrefabs.Length);
                SpawnEnemy(enemyPrefabs[index]);
                enemiesLeftToSpawn--;
                timeSinceLastSpawn = 0f;
            }

            yield return null;
        }
    }

    private void SpawnEnemy(GameObject prefab)
    {
        Instantiate(prefab, LevelManager.main.startPoint.position, Quaternion.identity);
        enemiesAlive++;
    }

    private void EndWave()
    {
        isSpawning = true; // prevent Update() calling EndWave repeatedly
        currentWave++;

        if (currentWave >= waves.Count && !victoryTriggered)
        {
            victoryTriggered = true;
            StartCoroutine(LoadVictoryScene());
            return;
        }

        StartCoroutine(StartWave());
    }

    private IEnumerator LoadVictoryScene()
    {
        Debug.Log($"Victory! Loading {victorySceneName} in {victoryDelay} seconds...");
        yield return new WaitForSeconds(victoryDelay);
        SceneManager.LoadScene(victorySceneName);
    }

    private int EnemiesPerWave()
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave + 1, difficultyScalingFactor));
    }

    private float EnemiesPerSecond()
    {
        return Mathf.Clamp(enemiesPerSecond * Mathf.Pow(currentWave + 1, difficultyScalingFactor), 0f, enemiesPerSecondCap);
    }
}