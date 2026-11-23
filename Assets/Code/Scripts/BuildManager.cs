using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BuildManager : MonoBehaviour
{
    public static BuildManager main;

    [Header("References")]
    [SerializeField] private SpriteRenderer previewRenderer; // empty GameObject with a SpriteRenderer
    [SerializeField] private Sprite crucifixIcon;
    [SerializeField] private float dragThreshold = 15f;      // pixels before a press counts as a drag

    [Header("Testing (used only if no loadout was committed)")]
    [SerializeField] private TowerDefinition[] testTowers;

    private LevelLoadout.Entry selected;
    private readonly Dictionary<string, float> cooldownEnd = new Dictionary<string, float>();
    private bool isHolding;
    private bool isDragging;
    private Plot hovered;
    private Vector2 pressStart;

    public LevelLoadout.Entry Selected => selected;
    public bool ExercismActive { get; private set; }

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
        ClearCrucifix();
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
        bool active = selected != null || ExercismActive;

        if (!active)
        {
            UpdateHover(Vector3.zero, false); // clears any leftover highlight
            return;
        }

        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        world.z = 0f;
        previewRenderer.transform.position = world;

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
            ClearCrucifix();
            return;
        }

        bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        UpdateHover(world, !overUI);

        // click-then-click mode: the tool is already selected, so this click targets a plot
        if (Input.GetMouseButtonDown(0) && !isHolding && !overUI)
        {
            Plot clicked = PlotAt(world);
            if (clicked != null) OnPlotClicked(clicked);
            return;
        }

        if (isHolding && !isDragging &&
            Vector2.Distance(pressStart, Input.mousePosition) > dragThreshold)
            isDragging = true;

        if (isHolding && Input.GetMouseButtonUp(0))
        {
            isHolding = false;

            if (isDragging)
            {
                Plot plot = PlotAt(world);
                if (plot != null)
                {
                    if (ExercismActive) plot.TryExercise();
                    else plot.TryBuild(selected);
                }
                ClearSelection();
                ClearCrucifix();
            }
            // a click leaves the tool selected, waiting for a plot click
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
        if (ExercismActive)
        {
            if (plot.TryExercise()) ClearCrucifix();   // one use per pick-up, like placing a ghost
            return;
        }

        if (selected == null) return;
        if (plot.TryBuild(selected)) ClearSelection();
    }

    private Plot PlotAt(Vector3 world)
    {
        foreach (Collider2D c in Physics2D.OverlapPointAll(world))
        {
            Plot p = c.GetComponent<Plot>();
            if (p != null) return p;
        }
        return null;
    }

    private void UpdateHover(Vector3 world, bool active)
    {
        Plot p = active ? PlotAt(world) : null;
        if (p == hovered) return;

        if (hovered != null) hovered.SetHover(false);
        hovered = p;
        if (hovered != null) hovered.SetHover(true);
    }

    public void BeginExercism()
    {
        ClearSelection();                    // drop any selected ghost
        ExercismActive = true;
        isHolding = true;
        isDragging = false;
        pressStart = Input.mousePosition;
        previewRenderer.sprite = crucifixIcon;
        previewRenderer.gameObject.SetActive(true);
    }

    public void ClearCrucifix()
    {
        ExercismActive = false;
        isHolding = false;
        isDragging = false;
        previewRenderer.gameObject.SetActive(false);
    }
}