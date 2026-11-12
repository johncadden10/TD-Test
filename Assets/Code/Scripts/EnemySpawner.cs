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

    [Header("Modifiers")]
    public bool belted;
    public bool fused;
    public bool constellation;
}

[System.Serializable]
public class Wave
{
    public string waveName;
    public bool useCustomWave;       // false = auto-scaling fallback
    public float duration = 0f;      // seconds until the next wave is sent, 0 = use the default
    public List<EnemyGroup> enemyGroups;
    public float timeBetweenGroups;
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    [Header("References")]
    [SerializeField] private GameObject[] enemyPrefabs;   // used by auto-scaled waves
    [SerializeField] private ModifierVisuals modifierVisuals;

    [Header("Effects")]
    [SerializeField] private GameObject defaultDeathEffect;
    public GameObject DefaultDeathEffect => defaultDeathEffect;

    [Header("Waves")]
    [SerializeField] private List<Wave> waves;

    [Header("Timing")]
    [SerializeField] private float firstWaveDelay = 10f;
    [SerializeField] private float defaultWaveDuration = 30f;
    [SerializeField] private float clearedDelay = 5f;     // pause after clearing a wave early

    [Header("Auto Scaling (fallback)")]
    [SerializeField] private int baseEnemies = 8;
    [SerializeField] private float enemiesPerSecond = 0.5f;
    [SerializeField] private float enemiesPerSecondCap = 8f;
    [SerializeField] private float difficultyScalingFactor = 0.75f;

    [Header("Victory")]
    [SerializeField] private string victorySceneName = "VictoryScene";
    [SerializeField] private float victoryDelay = 3f;

    private int nextWaveIndex;
    private float countdown;
    private int enemiesAlive;
    private int activeSpawns;
    private bool victoryTriggered;

    // for UI
    public int WavesLaunched => nextWaveIndex;
    public int TotalWaves => waves.Count;
    public float Countdown => Mathf.Max(0f, countdown);
    public bool AllWavesLaunched => nextWaveIndex >= waves.Count;

    private void Awake()
    {
        Instance = this;
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void OnDestroy()
    {
        onEnemyDestroy.RemoveListener(EnemyDestroyed);
        if (Instance == this) Instance = null;
    }

    private void Start()
    {
        countdown = firstWaveDelay;
    }

    private void Update()
    {
        if (victoryTriggered) return;

        bool cleared = activeSpawns == 0 && enemiesAlive <= 0;

        if (AllWavesLaunched)
        {
            if (cleared)
            {
                victoryTriggered = true;
                StartCoroutine(LoadVictoryScene());
            }
            return;
        }

        // wave cleared early: shorten the countdown (not before the first wave)
        if (cleared && nextWaveIndex > 0 && countdown > clearedDelay)
            countdown = clearedDelay;

        countdown -= Time.deltaTime;
        if (countdown <= 0f) LaunchNextWave();
    }

    // skips the countdown (call-early button, testing)
    public void CallNextWave()
    {
        if (!AllWavesLaunched) countdown = 0f;
    }

    private void LaunchNextWave()
    {
        int index = nextWaveIndex;
        Wave wave = waves[index];
        nextWaveIndex++;

        countdown = wave.duration > 0f ? wave.duration : defaultWaveDuration;

        activeSpawns++;
        StartCoroutine(RunWave(wave, index));
    }

    private IEnumerator RunWave(Wave wave, int index)
    {
        Debug.Log($"Launching wave {index + 1}: {wave.waveName}");

        if (wave.useCustomWave) yield return SpawnCustomWave(wave);
        else yield return SpawnAutoWave(index);

        activeSpawns--;
    }

    private IEnumerator SpawnCustomWave(Wave wave)
    {
        for (int g = 0; g < wave.enemyGroups.Count; g++)
        {
            EnemyGroup group = wave.enemyGroups[g];
            Health previous = null;

            for (int i = 0; i < group.count; i++)
            {
                GameObject star = SpawnEnemy(group.enemyPrefab);
                StarModifiers.Apply(star, group, previous, modifierVisuals);
                previous = star.GetComponent<Health>();

                // no trailing waits, so the wave counts as fully spawned right after the last star
                if (i < group.count - 1)
                    yield return new WaitForSeconds(group.spawnInterval);
            }

            if (g < wave.enemyGroups.Count - 1)
                yield return new WaitForSeconds(wave.timeBetweenGroups);
        }
    }

    private IEnumerator SpawnAutoWave(int waveIndex)
    {
        int left = EnemiesPerWave(waveIndex);
        float eps = EnemiesPerSecond(waveIndex);
        float timer = 0f;

        while (left > 0)
        {
            timer += Time.deltaTime;

            if (timer >= 1f / eps)
            {
                SpawnEnemy(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
                left--;
                timer = 0f;
            }

            yield return null;
        }
    }

    private GameObject SpawnEnemy(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, LevelManager.main.startPoint.position, Quaternion.identity);
        enemiesAlive++;
        return obj;
    }

    // used by SpawnOnDeath so dropped stars are counted
    public static void RegisterSpawnedEnemy()
    {
        if (Instance != null) Instance.enemiesAlive++;
    }

    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    private IEnumerator LoadVictoryScene()
    {
        Debug.Log($"Victory! Loading {victorySceneName} in {victoryDelay} seconds...");
        yield return new WaitForSeconds(victoryDelay);
        SceneManager.LoadScene(victorySceneName);
    }

    private int EnemiesPerWave(int waveIndex)
    {
        return Mathf.RoundToInt(baseEnemies * Mathf.Pow(waveIndex + 1, difficultyScalingFactor));
    }

    private float EnemiesPerSecond(int waveIndex)
    {
        return Mathf.Clamp(enemiesPerSecond * Mathf.Pow(waveIndex + 1, difficultyScalingFactor), 0f, enemiesPerSecondCap);
    }
}