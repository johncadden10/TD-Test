using UnityEngine;

public abstract class GhostModifier : MonoBehaviour
{
    protected Ghost ghost;

    public void Attach(Ghost g)
    {
        ghost = g;
        OnAttached();
    }

    protected abstract void OnAttached();
}