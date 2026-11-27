using UnityEngine;
using UnityEngine.EventSystems;

public class SoulCollector : MonoBehaviour
{
    [SerializeField] private float collectRadius = 0.15f;

    private bool holding;
    private Vector2 lastPos;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private Vector2 MouseWorld()
    {
        return cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Update()
    {
        if (GameSpeed.IsPaused) { holding = false; return; }
        if (Input.GetMouseButtonDown(0))
        {
            bool placing = BuildManager.main != null && (BuildManager.main.Selected != null || BuildManager.main.ExercismActive);
            bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

            // a press that starts on UI or while placing a ghost never becomes a sweep
            holding = !placing && !overUI;
            lastPos = MouseWorld();
        }

        if (Input.GetMouseButtonUp(0)) holding = false;
        if (!holding) return;

        if (BuildManager.main != null && BuildManager.main.Selected != null)
        {
            holding = false;
            return;
        }

        Vector2 pos = MouseWorld();
        Vector2 delta = pos - lastPos;

        RaycastHit2D[] hits;
        if (delta.sqrMagnitude < 0.0001f)
        {
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(pos, collectRadius);
            foreach (Collider2D c in overlaps) TryCollect(c);
        }
        else
        {
            hits = Physics2D.CircleCastAll(lastPos, collectRadius, delta.normalized, delta.magnitude);
            foreach (RaycastHit2D hit in hits) TryCollect(hit.collider);
        }

        lastPos = pos;
    }

    private static void TryCollect(Collider2D c)
    {
        Soul soul = c.GetComponentInParent<Soul>();
        if (soul != null) soul.Collect();
    }
}