using UnityEngine;
using UnityEngine.EventSystems;

public class Soul : MonoBehaviour
{
    [SerializeField] private float lifetime = 6f;

    private int value;
    private VengefulSettings vengeful;
    private bool finished;

    public void Init(int soulValue, VengefulSettings settings)
    {
        value = soulValue;
        vengeful = settings; // shared reference, so it still works if the reaper dies first
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f) Expire();
    }

    public void Collect()
    {
        if (finished) return;
        finished = true;
        LevelManager.main.IncreaseEcto(value);
        Destroy(gameObject);
    }

    private void Expire()
    {
        if (finished) return;
        finished = true;

        if (vengeful != null && vengeful.enabled && vengeful.prefab != null)
        {
            GameObject obj = Instantiate(vengeful.prefab, transform.position, Quaternion.identity);
            obj.GetComponent<VengefulSoul>().Init(vengeful);
        }
        Destroy(gameObject);
    }
}