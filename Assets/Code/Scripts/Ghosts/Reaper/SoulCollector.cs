using UnityEngine;
using UnityEngine.EventSystems;

public class SoulCollector : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (BuildManager.main != null && BuildManager.main.Selected != null) return; // placing a ghost
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (Collider2D c in Physics2D.OverlapPointAll(p))
        {
            Soul soul = c.GetComponent<Soul>();
            if (soul != null) soul.Collect();
        }
    }
}