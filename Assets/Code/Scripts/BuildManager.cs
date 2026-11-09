using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private SpriteRenderer previewRenderer; // empty GameObject with a SpriteRenderer
    [SerializeField] private float dragThreshold = 15f;      // pixels before a press counts as a drag

    [Header("Testing (used only if no loadout was committed)")]
    [SerializeField] private TowerDefinition[] testTowers;

    private LevelLoadout.Entry selected;
    private readonly Dictionary<string, float> cooldownEnd = new Dictionary<string, float>();
    private bool isHolding;
    private bool isDragging;
    private Vector2 pressStart;

    public LevelLoadout.Entry Selected => selected;

    private void Awake()
    {
        main = this;
        previewRenderer.gameObject.SetActive(false);

        // lets you press Play directly in a level scene
        if (LevelLoadout.entries.Count == 0 && testTowers != null)
        {
            foreach (TowerDefinition t in testTowers)
                LevelLoadout.entries.Add(new LevelLoadout.Entry { tower = t, pathIndex = 0 });
        }
    }

    // called by a build button on pointer down
    public void BeginSelect(LevelLoadout.Entry entry)
    {
        selected = entry;
        isHolding = true;
        isDragging = false;
        pressStart = Input.mousePosition;

        previewRenderer.sprite = entry.tower.icon;
        previewRenderer.gameObject.SetActive(true);
    }

    public void ClearSelection()
    {
        selected = null;
        isHolding = false;
        isDragging = false;
        previewRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (selected == null) return;

        // preview follows the mouse
        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        world.z = 0f;
        previewRenderer.transform.position = world;

        // cancel
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
            return;
        }

        // detect drag
        if (isHolding && !isDragging &&
            Vector2.Distance(pressStart, Input.mousePosition) > dragThreshold)
            isDragging = true;

        // release
        if (isHolding && Input.GetMouseButtonUp(0))
        {
            isHolding = false;

            if (isDragging)
            {
                // drag mode: drop onto whatever plot is under the mouse
                Plot plot = GetPlotUnderMouse(world);
                if (plot != null) plot.TryBuild(selected);
                ClearSelection(); // dropping always ends the action
            }
            // else: it was a click, so stay selected and wait for a plot click
        }
    }

    public float CooldownRemaining(LevelLoadout.Entry entry)
    {
        if (cooldownEnd.TryGetValue(entry.tower.towerId, out float end))
            return Mathf.Max(0f, end - Time.time);
        return 0f;
    }

    public bool IsOnCooldown(LevelLoadout.Entry entry) => CooldownRemaining(entry) > 0f;

    public float CooldownFraction(LevelLoadout.Entry entry)
    {
        float total = entry.tower.placementCooldown;
        return total <= 0f ? 0f : Mathf.Clamp01(CooldownRemaining(entry) / total);
    }

    public void StartCooldown(LevelLoadout.Entry entry)
    {
        if (entry.tower.placementCooldown > 0f)
            cooldownEnd[entry.tower.towerId] = Time.time + entry.tower.placementCooldown;
    }

    // called by Plot on click when something is selected (click-click mode)
    public void OnPlotClicked(Plot plot)
    {
        if (selected == null) return;
        if (plot.TryBuild(selected))
            ClearSelection();
    }

    private Plot GetPlotUnderMouse(Vector3 world)
    {
        Collider2D hit = Physics2D.OverlapPoint(world);
        return hit != null ? hit.GetComponent<Plot>() : null;
    }
}