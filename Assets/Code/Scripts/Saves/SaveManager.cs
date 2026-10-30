using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager main;

    private const int SAVE_SLOTS = 3;
    private const string SAVE_PREFIX = "save_slot_";
    private const int DefaultSlot = 0; // slot used when nothing was chosen in the menu

    private SaveData currentSave;
    public int CurrentSlot { get; private set; } = -1;

    // loads the default slot automatically if nobody has chosen one yet
    public SaveData CurrentSave
    {
        get
        {
            if (currentSave == null)
            {
                Debug.Log($"No slot selected, auto-loading slot {DefaultSlot}");
                LoadSlot(DefaultSlot);
            }
            return currentSave;
        }
    }

    // creates a SaveManager if the scene you started from doesn't have one
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (main == null)
            new GameObject("SaveManager").AddComponent<SaveManager>();
    }

    private void Awake()
    {
        if (main != null)
        {
            Destroy(gameObject);
            return;
        }
        main = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSlot(int slot)
    {
        CurrentSlot = slot; // must be set before any save call

        string path = GetPath(slot);
        if (File.Exists(path))
        {
            currentSave = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }
        else
        {
            currentSave = new SaveData();
            currentSave.saveName = $"Save {slot + 1}";
            SaveCurrentSlot();
        }
    }

    public void SaveCurrentSlot()
    {
        if (CurrentSlot < 0 || currentSave == null) return;
        File.WriteAllText(GetPath(CurrentSlot), JsonUtility.ToJson(currentSave, true));
    }

    public void DeleteSlot(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path)) File.Delete(path);
    }

    public SaveData PeekSlot(int slot)
    {
        string path = GetPath(slot);
        return File.Exists(path) ? JsonUtility.FromJson<SaveData>(File.ReadAllText(path)) : null;
    }

    public bool SlotExists(int slot) => File.Exists(GetPath(slot));

    private string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"{SAVE_PREFIX}{slot}.json");
    }
}