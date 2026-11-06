using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSpeed
{
    public static float Speed { get; private set; } = 1f;
    private static int pauseCount;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        Speed = 1f;
        pauseCount = 0;
        Apply();
        SceneManager.sceneLoaded += (s, m) => { pauseCount = 0; Apply(); }; // a new scene is never paused
    }

    public static void SetSpeed(float speed)
    {
        Speed = Mathf.Clamp(speed, 0.25f, 8f);
        Apply();
    }

    public static void Pause()  { pauseCount++; Apply(); }
    public static void Resume() { pauseCount = Mathf.Max(0, pauseCount - 1); Apply(); }
    public static void ClearPauses() { pauseCount = 0; Apply(); }

    private static void Apply()
    {
        Time.timeScale = pauseCount > 0 ? 0f : Speed;
    }
}