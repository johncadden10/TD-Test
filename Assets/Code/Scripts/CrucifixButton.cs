using UnityEngine;
using UnityEngine.EventSystems;

public class CrucifixButton : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameSpeed.IsPaused) return;
        BuildManager.main.BeginExercism();
    }

    private void Update()
    {
        if (!KeybindManager.Pressed(KeybindAction.Crucifix)) return;

        // pressing the key again puts the shovel away
        if (BuildManager.main.ExercismActive) BuildManager.main.CancelTool();
        else BuildManager.main.BeginExercism(true);
    }
}
