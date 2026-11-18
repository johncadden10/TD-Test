using UnityEngine;

public class GraveTile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Plot bottomPlot;
    [SerializeField] private Soul soulPrefab;
    [SerializeField] private Transform soulSpawnPoint;   // e.g. the top section
    [SerializeField] private GameObject generatingVisual; // child of the top section, shown while active
    [SerializeField] private Animator animator;           // optional, bool parameter "Generating"

    [Header("Attributes")]
    [SerializeField] private int soulValue = 15;
    [SerializeField] private float interval = 8f;

    private static readonly int GeneratingHash = Animator.StringToHash("Generating");

    private float progress;
    private bool generating;

    private void Start()
    {
        SetGenerating(false);
    }

    private void Update()
    {
        bool active = HasLiveGhost();

        if (active != generating) SetGenerating(active);
        if (!active) return; // progress is kept, so the timer resumes where it stopped

        progress += Time.deltaTime;

        if (progress >= interval)
        {
            progress -= interval;
            SpawnSoul();
        }
    }

    private bool HasLiveGhost()
    {
        Ghost g = bottomPlot.Occupant;
        return g != null && g.IsAlive;
    }

    private void SpawnSoul()
    {
        Vector2 origin = soulSpawnPoint != null ? (Vector2)soulSpawnPoint.position : (Vector2)transform.position;
        Vector2 pos = origin;

        Soul soul = Instantiate(soulPrefab, pos, Quaternion.identity);
        soul.Init(soulValue, null); // plain soul: no vengeful behaviour
    }

    private void SetGenerating(bool value)
    {
        generating = value;
        if (generatingVisual != null) generatingVisual.SetActive(value);
        if (animator != null) animator.SetBool(GeneratingHash, value);
    }
}