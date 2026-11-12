using UnityEngine;

public class BansheeAttack : MonoBehaviour
{
    [SerializeField] private GameObject effectObject;

    private Vector3 baseScale;
    private bool baseCaptured;

    public void SetActive(bool active)
    {
        effectObject.SetActive(active);
    }

    // multiplier of 1 = the size you authored for the base radius
    public void SetScaleMultiplier(float multiplier)
    {
        if (!baseCaptured)
        {
            baseScale = effectObject.transform.localScale;
            baseCaptured = true;
        }
        effectObject.transform.localScale = baseScale * multiplier;
    }
}