using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Shared Info Display")]
    [SerializeField] private GameObject emptyInfoObject;   // "No save data" / "New Game"
    [SerializeField] private GameObject saveInfoObject;    // parent of the three texts below
    [SerializeField] private TextMeshProUGUI slotNameText;
    [SerializeField] private TextMeshProUGUI essenceText;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Buttons")]
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject deleteButton;

    [Header("Other")]
    [SerializeField] private GameObject confirmDeletePanel;
    [SerializeField] private string hubSceneName = "Hub";
    [SerializeField] private GameObject infoDisplayParent;

    [Header("Slot Highlight")]
    [SerializeField] private Image[] slotButtonImages; // Slot1, Slot2, Slot3 button images
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.cyan;

    private int selectedSlot = -1;

    private void HighlightSlot(int slot)
    {
        for (int i = 0; i < slotButtonImages.Length; i++)
            slotButtonImages[i].color = (i == slot) ? selectedColor : normalColor;
    }

    private void OnEnable()
    {
        ClearSelection();
    }

    private void ClearSelection()
    {
        selectedSlot = -1;
        HighlightSlot(-1);
        infoDisplayParent.SetActive(false);
        emptyInfoObject.SetActive(false);
        saveInfoObject.SetActive(false);
        playButton.SetActive(false);
        deleteButton.SetActive(false);
    }

    // wired from each slot button with an int argument (0, 1, 2)
    public void SelectSlot(int slot)
    {
        selectedSlot = slot;
        HighlightSlot(slot);
        infoDisplayParent.SetActive(true);
        playButton.SetActive(true);

        SaveData data = SaveManager.main.PeekSlot(slot);

        if (data == null)
        {
            emptyInfoObject.SetActive(true);
            saveInfoObject.SetActive(false);
            deleteButton.SetActive(false); // nothing to delete
        }
        else
        {
            emptyInfoObject.SetActive(false);
            saveInfoObject.SetActive(true);
            deleteButton.SetActive(true);
            slotNameText.text = data.saveName;
            essenceText.text = $"Essence: {data.essence}";
            levelText.text = $"Level: {data.highestLevelUnlocked + 1}";
        }
    }

    public void OnPlayPressed()
    {
        if (selectedSlot < 0) return;
        SaveManager.main.LoadSlot(selectedSlot);
        SceneManager.LoadScene(hubSceneName);
    }

    public void OnDeletePressed()
    {
        if (selectedSlot < 0) return;
        confirmDeletePanel.SetActive(true);
    }

    public void OnConfirmDelete()
    {
        if (selectedSlot >= 0)
        {
            SaveManager.main.DeleteSlot(selectedSlot);
            SelectSlot(selectedSlot); // refresh display, now showing empty
        }
        confirmDeletePanel.SetActive(false);
    }

    public void OnCancelDelete()
    {
        confirmDeletePanel.SetActive(false);
    }
}