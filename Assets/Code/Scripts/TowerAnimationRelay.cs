using UnityEngine;

public class TowerAnimationRelay : MonoBehaviour
{
    private Ghost tower;

    private void Awake()
    {
        tower = GetComponentInParent<Ghost>();
    }

    public void OnDeathEnd()
    {
        tower.OnDeathEnd();
    }

    public void OnAttackFire()
    {
        tower.OnAttackFire();
    }

    public void OnAttackEnd()
    {
        tower.OnAttackEnd();
    }
}