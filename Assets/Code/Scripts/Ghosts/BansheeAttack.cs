using UnityEngine;

public class BansheeAttack : MonoBehaviour
{
    [SerializeField] private GameObject effectObject;

    public void SetActive(bool active)
    {
        effectObject.SetActive(active);
    }
}