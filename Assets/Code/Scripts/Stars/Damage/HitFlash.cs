using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private Material flashMaterial;       // the SpriteFlash material
    [SerializeField] private float flashDuration = 0.12f;
    [SerializeField, Range(0f, 1f)] private float strength = 0.9f;
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private bool skipStatusDamage = true;

    private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");
    private static readonly int FlashColor = Shader.PropertyToID("_FlashColor");

    private SpriteRenderer[] renderers = new SpriteRenderer[0];
    private Material[] instances = new Material[0];
    private Health health;
    private float timer;
    private bool dirty;

    private void Awake()
    {
        health = GetComponent<Health>();
        CacheRenderers();
    }

    private void OnEnable()  { health.OnDamaged += OnDamaged; }
    private void OnDisable() { health.OnDamaged -= OnDamaged; }

    private void OnDestroy() { DestroyInstances(); }

    // call again after adding sprites at runtime, such as the Belted overlay
    public void CacheRenderers()
    {
        if (flashMaterial == null) return;

        DestroyInstances();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        instances = new Material[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            instances[i] = new Material(flashMaterial);
            instances[i].SetColor(FlashColor, flashColor);
            instances[i].SetFloat(FlashAmount, 0f);
            renderers[i].sharedMaterial = instances[i];
        }
    }

    private void DestroyInstances()
    {
        foreach (Material m in instances)
            if (m != null) Destroy(m);
        instances = new Material[0];
    }

    private void OnDamaged(DamageInfo info)
    {
        if (skipStatusDamage && info.fromStatus) return;
        timer = flashDuration;
        dirty = true;
    }

    private void Update()
    {
        if (!dirty) return;

        timer -= Time.deltaTime;
        float amount = timer > 0f ? strength * (timer / flashDuration) : 0f;
        if (timer <= 0f) dirty = false; // one last write of 0 below, then idle

        foreach (Material m in instances)
            if (m != null) m.SetFloat(FlashAmount, amount);
    }
}