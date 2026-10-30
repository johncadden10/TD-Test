using UnityEngine;

// Path 2-2
public class AdjacencyModifier : ReaperModifier
{
    [SerializeField] private float radius = 1.5f;
    [SerializeField] private float bonusPerNeighbor = 0.25f;
    [SerializeField] private float checkInterval = 0.5f;

    protected override void Apply() { InvokeRepeating(nameof(Recalculate), 0f, checkInterval); }

    private void Recalculate()
    {
        int neighbors = 0;
        foreach (Reaper other in Reaper.All)
            if (other != reaper && Vector2.Distance(other.transform.position, reaper.transform.position) <= radius)
                neighbors++;

        reaper.AdjacencyMultiplier = 1f + neighbors * bonusPerNeighbor;
        Debug.Log($"Adjacency: {neighbors} neighbors, x{reaper.AdjacencyMultiplier}");
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
