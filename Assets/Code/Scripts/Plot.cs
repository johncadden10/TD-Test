using UnityEngine;
using UnityEngine.EventSystems;

public class Plot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hoverColor;
    [SerializeField] private Color crucifixHoverColor = new Color(1f, 0.4f, 0.4f);

    private GameObject tower;
    private Color startColor;
    private Ghost occupant;
    public Ghost Occupant => occupant;

    private void Start()
    {
        startColor = sr.color;
    }

    // called by BuildManager, which tracks the hovered plot itself
    public void SetHover(bool on)
    {
        if (!on) { sr.color = startColor; return; }

        bool shovel = BuildManager.main.ExercismActive;
        bool build = BuildManager.main.Selected != null;

        if (shovel && tower != null) sr.color = crucifixHoverColor;
        else if (build && tower == null) sr.color = hoverColor;
    }

    public bool TryBuild(LevelLoadout.Entry entry)
    {
        if (tower != null) return false;
        if (BuildManager.main.IsOnCooldown(entry)) return false;

        if (entry.tower.cost > LevelManager.main.ecto)
        {
            Debug.Log("Insufficient Ecto!");
            return false;
        }

        LevelManager.main.SpendEcto(entry.tower.cost);

        tower = Instantiate(entry.tower.prefab, transform.position, Quaternion.identity);

        Ghost ghost = tower.GetComponent<Ghost>();
        occupant = ghost;
        if (ghost != null) ghost.Initialize(entry.tower.towerId, entry.pathIndex);

        BuildManager.main.StartCooldown(entry); // starts when placed, not when picked up

        sr.color = startColor;
        return true;
    }

    public bool TryExercise()
    {
        if (tower == null) return false;

        Ghost g = tower.GetComponent<Ghost>();
        if (g != null) g.Exercise();
        else Destroy(tower);

        tower = null;
        occupant = null;
        sr.color = startColor;
        return true;
    }
}