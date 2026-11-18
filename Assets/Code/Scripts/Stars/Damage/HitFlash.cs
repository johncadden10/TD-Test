using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;       // the SpriteFlash material
    [SerializeField] private float flashDuration = 0.12f;
    [SerializeField, Range(0f, 1f)] private float strength = 0.7f;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private bool skipStatusDamage = true; // burn ticks don't flash

    private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
    private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");

    private SpriteRenderer[] renderers;
    private MaterialPropertyBlock block;
    private Health health;
    private float timer;

    private void Awake()
    {
        health = GetComponent<Health>();
        block = new MaterialPropertyBlock();
        CacheRenderers();
    }

    private void OnEnable()  { health.OnDamaged += OnDamaged; }
    private void OnDisable() { health.OnDamaged -= OnDamaged; }

    // call again if you add sprites at runtime, such as the Belted overlay
    public void CacheRenderers()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        if (flashMaterial == null) return;
        foreach (SpriteRenderer sr in renderers)
            sr.sharedMaterial = flashMaterial;
    }

    private void OnDamaged(DamageInfo info)
    {
        if (skipStatusDamage && info.fromStatus) return;
        timer = flashDuration;
    }

    private void Update()
    {
        if (timer <= 0f) return;

        timer -= Time.deltaTime;
        float amount = timer > 0f ? strength * (timer / flashDuration) : 0f;

        foreach (SpriteRenderer sr in renderers)
        {
            if (sr == null) continue;
            sr.GetPropertyBlock(block);
            block.SetFloat(FlashAmount, amount);
            block.SetColor(FlashColor, flashColor);
            sr.SetPropertyBlock(block);
        }
    }
}