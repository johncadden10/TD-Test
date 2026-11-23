using UnityEngine;
using UnityEngine.EventSystems;

public class CrucifixButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        BuildManager.main.BeginExercism();
    }
}
