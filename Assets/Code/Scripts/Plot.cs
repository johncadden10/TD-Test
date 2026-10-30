using UnityEngine;
using UnityEngine.EventSystems;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;

    private GameObject tower;
    private Color startColor;

    private void Start()
    {
        startColor = sr.color;
    }

    private void OnMouseEnter()
    {
        // only highlight when something is being placed, and the plot is free
        if (BuildManager.main.Selected != null && tower == null)
            sr.color = hoverColor;
    }

    private void OnMouseExit()
    {
        sr.color = startColor;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // click was on UI
        BuildManager.main.OnPlotClicked(this);
    }

    public bool TryBuild(LevelLoadout.Entry entry)
    {
        if (tower != null) return false;

        if (entry.tower.cost > LevelManager.main.ecto)
        {
            Debug.Log("Insufficient Ecto!");
            return false;
        }

        LevelManager.main.SpendEcto(entry.tower.cost);

        tower = Instantiate(entry.tower.prefab, transform.position, Quaternion.identity);

        Ghost ghost = tower.GetComponent<Ghost>();
        if (ghost != null) ghost.Initialize(entry.tower.towerId, entry.pathIndex);

        sr.color = startColor;
        return true;
    }
}