using System;
using System.Collections.Generic;
using UnityEngine;

public class KeybindManager : MonoBehaviour
{
    public static event Action OnBindingsChanged;
    public static bool Rebinding { get; private set; }
    public static int LastRebindFrame { get; private set; } = -1;

    private static KeybindManager instance;
    private static readonly Dictionary<KeybindAction, KeyCode> bindings = new Dictionary<KeybindAction, KeyCode>();
    private static bool loaded;

    private static readonly Dictionary<KeybindAction, KeyCode> defaults = new Dictionary<KeybindAction, KeyCode>
    {
        { KeybindAction.BuildSlot1, KeyCode.Q }, { KeybindAction.BuildSlot2, KeyCode.W },
        { KeybindAction.BuildSlot3, KeyCode.E }, { KeybindAction.BuildSlot4, KeyCode.R },
        { KeybindAction.UpgradeSlot1, KeyCode.Alpha1 },    { KeybindAction.UpgradeSlot2, KeyCode.Alpha2 },
        { KeybindAction.UpgradeSlot3, KeyCode.Alpha3 },    { KeybindAction.UpgradeSlot4, KeyCode.Alpha4 },
        { KeybindAction.Crucifix, KeyCode.LeftShift },
        { KeybindAction.GameSpeed, KeyCode.Space },
    };

    private static readonly KeyCode[] allKeys = (KeyCode[])Enum.GetValues(typeof(KeyCode));

    private KeybindAction pendingAction;
    private Action onRebindDone;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null) return;
        GameObject go = new GameObject("KeybindManager");
        instance = go.AddComponent<KeybindManager>();
        DontDestroyOnLoad(go);
    }

    // ---- reading bindings ----
    private static void Load()
    {
        if (loaded) return;
        loaded = true;

        foreach (KeybindAction a in Enum.GetValues(typeof(KeybindAction)))
        {
            KeyCode key = defaults[a];
            string saved = PlayerPrefs.GetString("keybind_" + a, "");
            if (saved != "" && Enum.TryParse(saved, out KeyCode parsed)) key = parsed;
            bindings[a] = key;
        }
    }

    public static KeyCode Get(KeybindAction action)
    {
        Load();
        return bindings[action];
    }

    // true on the frame the key goes down (never while paused or rebinding)
    public static bool Pressed(KeybindAction action)
    {
        if (Rebinding || GameSpeed.IsPaused) return false;
        KeyCode key = Get(action);
        return key != KeyCode.None && Input.GetKeyDown(key);
    }

    public static string KeyLabel(KeybindAction action)
    {
        KeyCode key = Get(action);
        if (key == KeyCode.None) return "-";
        return key.ToString().Replace("Alpha", "").Replace("Keypad", "Num ");
    }

    // ---- changing bindings ----
    public static void Set(KeybindAction action, KeyCode key)
    {
        Load();
        KeyCode old = bindings[action];

        // another action already uses this key: swap, so no two actions share a key
        foreach (KeybindAction other in new List<KeybindAction>(bindings.Keys))
        {
            if (other != action && bindings[other] == key)
                bindings[other] = old;
        }

        bindings[action] = key;
        Save();
    }

    public static void ResetAll()
    {
        Load();
        foreach (var pair in defaults) bindings[pair.Key] = pair.Value;
        Save();
    }

    private static void Save()
    {
        foreach (var pair in bindings)
            PlayerPrefs.SetString("keybind_" + pair.Key, pair.Value.ToString());
        PlayerPrefs.Save();
        OnBindingsChanged?.Invoke();
    }

    // ---- listening for the next key ----
    public static void StartRebind(KeybindAction action, Action onDone)
    {
        if (instance == null) return;
        instance.pendingAction = action;
        instance.onRebindDone = onDone;
        Rebinding = true;
    }

    public static void CancelRebind()
    {
        Rebinding = false;
        if (instance != null) instance.onRebindDone = null;
    }

    private void Update()
    {
        if (!Rebinding) return;

        foreach (KeyCode key in allKeys)
        {
            if (key == KeyCode.None || key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6) continue;
            if (!Input.GetKeyDown(key)) continue;

            if (key != KeyCode.Escape) Set(pendingAction, key); // Escape cancels

            Rebinding = false;
            LastRebindFrame = Time.frameCount;
            onRebindDone?.Invoke();
            onRebindDone = null;
            return;
        }
    }
}