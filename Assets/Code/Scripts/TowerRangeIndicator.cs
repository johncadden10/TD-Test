using UnityEngine;

public class TowerRangeIndicator : MonoBehaviour
{
    [SerializeField] private GameObject rangeCircle;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        rangeCircle.SetActive(false);

        Ghost ghost = GetComponent<Ghost>();
        if (ghost != null)
        {
            float diameter = ghost.TargetingRange * 2f;
            rangeCircle.transform.localScale = new Vector3(diameter, diameter, 1f);
        }
    }

    private void Update()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D col = GetComponent<Collider2D>();
        bool isHovering = col.OverlapPoint(mousePos);
        rangeCircle.SetActive(isHovering);
    }
}