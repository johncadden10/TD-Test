using UnityEngine;

[DefaultExecutionOrder(-100)]
public class LevelBootstrap : MonoBehaviour
{
    [Header("Used only when pressing Play directly in the Level scene")]
    [SerializeField] private GameObject testLayout;
    [SerializeField] private WaveSet testWaves;
    [SerializeField] private int testStartingEcto = 125;

    [Header("References")]
    [SerializeField] private EnemySpawner spawner;

    private void Awake()
    {
        GameObject layoutPrefab = testLayout;
        WaveSet waves = testWaves;
        int ecto = testStartingEcto;

        LevelDefinition level = LevelSession.current;
        if (level != null)
        {
            layoutPrefab = level.layoutPrefab;
            waves = level.waves;
            ecto = level.startingEcto;
        }

        if (layoutPrefab == null || waves == null)
        {
            Debug.LogError("LevelBootstrap: no layout or waves. Fill the level in the registry, or assign the test fields.");
            return;
        }

        LevelLayout layout = Instantiate(layoutPrefab).GetComponent<LevelLayout>();

        LevelManager.main.ApplyLayout(layout.GetPath(), ecto);
        spawner.SetWaves(waves.waves);

        if (Camera.main != null && Camera.main.orthographic)
            Camera.main.orthographicSize = layout.CameraSize;
    }
}