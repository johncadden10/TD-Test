using UnityEngine;

public class EctoTarget : MonoBehaviour
{
    public static EctoTarget main;

    [SerializeField] private RectTransform target;   // empty = this object
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseDecay = 8f;

    private Canvas canvas;
    private Vector3 baseScale;
    private float pulse;

    private void Awake()
    {
        main = this;
        if (target == null) target = (RectTransform)transform;
        canvas = GetComponentInParent<Canvas>();
        baseScale = target.localScale;
    }

    private void OnDestroy()
    {
        if (main == this) main = null;
    }

    // world position of the counter, recomputed on request because the menu slides
    public Vector3 WorldPoint()
    {
        Camera cam = Camera.main;
        Camera uiCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(uiCam, target.position);
        return cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, Mathf.Abs(cam.transform.position.z)));
    }

    public void Pulse() { pulse = 1f; }

    private void Update()
    {
        if (pulse <= 0f) return;
        pulse = Mathf.MoveTowards(pulse, 0f, pulseDecay * Time.unscaledDeltaTime);
        target.localScale = baseScale * (1f + (pulseScale - 1f) * pulse);
    }
}