/* using UnityEngine;
using TMPro;

public class SaveSlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI slotNameText;
    [SerializeField] private TextMeshProUGUI essenceText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject emptySlotObject; // "New Game" display
    [SerializeField] private GameObject saveDataObject;  // actual save info display
    [SerializeField] private SaveSlotUI slotUI;

    private int slotIndex;

    public void Refresh(int index, SaveData data)
    {
        slotIndex = index;
        Debug.Log($"Slot {index} refreshed, data: {(data == null ? "empty" : data.saveName)}");

        if (data == null)
        {
            emptySlotObject.SetActive(true);
            saveDataObject.SetActive(false);
        }
        else
        {
            emptySlotObject.SetActive(false);
            saveDataObject.SetActive(true);
            slotNameText.text = data.saveName;
            essenceText.text = $"Essence: {data.essence}";
            levelText.text = $"Level: {data.highestLevelUnlocked + 1}";
        }
    }

    public void OnSlotPressed()
    {
        slotUI.SelectSlot(slotIndex);
    }
}
*/