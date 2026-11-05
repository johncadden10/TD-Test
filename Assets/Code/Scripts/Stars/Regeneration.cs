using UnityEngine;

public class Regeneration : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private float healPerSecond = 5f;
    [SerializeField] private float fireSuppressTime = 3f;

    private Health health;
    private float suppressedUntil;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnDamaged += HandleDamaged;
    }

    private void OnDisable()
    {
        health.OnDamaged -= HandleDamaged;
    }

    private void HandleDamaged(DamageInfo info)
    {
        if (info.type.Has(DamageType.Fire))
            suppressedUntil = Time.time + fireSuppressTime;
    }

    private void Update()
    {
        if (Time.time < suppressedUntil) return;
        if (health.IsFull) return;

        health.Heal(healPerSecond * Time.deltaTime);
    }
}