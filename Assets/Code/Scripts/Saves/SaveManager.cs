using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager main;

    private const int SAVE_SLOTS = 3;
    private const string SAVE_PREFIX = "save_slot_";

    public SaveData CurrentSave { get; private set; }
    public int CurrentSlot { get; private set; } = -1;

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
    /*
    private void Start()
    {
        if (CurrentSlot < 0)
        {
            Debug.Log("No save slot loaded, auto-loading slot 0 for testing");
            LoadSlot(0);
        }
    } */

    public void LoadSlot(int slot)
    {
        CurrentSlot = slot; // must be set BEFORE any save call

        string path = GetPath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            CurrentSave = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            CurrentSave = new SaveData();
            CurrentSave.saveName = $"Save {slot + 1}";
            SaveCurrentSlot();
        }
    }

    public void SaveCurrentSlot()
    {
        if (CurrentSlot < 0) return;
        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(GetPath(CurrentSlot), json);
    }

    public void DeleteSlot(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path))
            File.Delete(path);
    }

    public SaveData PeekSlot(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path))
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        return null;
    }

    public bool SlotExists(int slot)
    {
        return File.Exists(GetPath(slot));
    }

    private string GetPath(int slot)
    {
        return Path.Combine(Application.persistentDataPath, $"{SAVE_PREFIX}{slot}.json");
    }
}