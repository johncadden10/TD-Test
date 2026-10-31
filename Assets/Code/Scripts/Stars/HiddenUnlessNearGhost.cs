using UnityEngine;

public class HiddenUnlessNearGhost : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float revealRadius = 1f;   // roughly one tile
    [SerializeField] private float fadeSpeed = 8f;      // 0 = instant
    [SerializeField] private bool untargetableWhenHidden = false;
    [SerializeField, Range(0f, 1f)] private float hiddenOpacity = 0.3f;

    private SpriteRenderer[] renderers;
    private Collider2D[] colliders;
    private Health health;
    private float alpha;
    private bool revealed;

    private void Awake()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        health = GetComponent<Health>();
    }

    private void Start()
    {
        alpha = hiddenOpacity;
        SetAlpha(alpha);
        health.IsTargetable = false;
    }

    private void Update()
    {
        revealed = IsNearGhost();
        health.IsTargetable = revealed;

        float target = revealed ? 1f : hiddenOpacity;
        alpha = fadeSpeed <= 0f ? target : Mathf.MoveTowards(alpha, target, fadeSpeed * Time.deltaTime);
        SetAlpha(alpha);
    }

    private bool IsNearGhost()
    {
        foreach (Ghost g in Ghost.Active)
        {
            if (!g.IsAlive) continue;
            if (Vector2.Distance(g.transform.position, transform.position) <= revealRadius)
                return true;
        }
        return false;
    }

    private void SetAlpha(float a)
    {
        foreach (SpriteRenderer sr in renderers)
        {
            Color c = sr.color;
            c.a = a;
            sr.color = c;
        }
    }

    private void ApplyColliders(bool enabled)
    {
        if (!untargetableWhenHidden) return;
        foreach (Collider2D c in colliders) c.enabled = enabled;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, revealRadius);
    }
}