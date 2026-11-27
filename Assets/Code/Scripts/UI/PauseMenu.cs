using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmExitPanel;

    [SerializeField] private string hubSceneName = "Hub";

    private bool open;

    private void Start()
    {
        HideAll();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        // the key was used to cancel a rebind
        if (KeybindManager.Rebinding || Time.frameCount == KeybindManager.LastRebindFrame) return;

        if (open) { Back(); return; }
        if (GameSpeed.IsPaused) return;                                      // e.g. the game over screen
        if (BuildManager.main != null && BuildManager.main.CancelTool()) return; // Escape cancels a held ghost or shovel first

        Open();
    }

    // wire to the top-right pause button
    public void Open()
    {
        if (open || GameSpeed.IsPaused) return;
        open = true;
        GameSpeed.Pause();
        HideAll();
        pausePanel.SetActive(true);
    }

    public void OnResumePressed()
    {
        if (!open) return;
        open = false;
        HideAll();
        GameSpeed.Resume();
    }

    public void OnSettingsPressed()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnSettingsBack()
    {
        KeybindManager.CancelRebind();
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void OnExitPressed()
    {
        confirmExitPanel.SetActive(true);
    }

    public void OnExitCancelled()
    {
        confirmExitPanel.SetActive(false);
    }

    public void OnExitConfirmed()
    {
        GameSpeed.ClearPauses();
        SceneManager.LoadScene(hubSceneName);
    }

    public void OnResetKeybindsPressed()
    {
        KeybindManager.ResetAll();
    }

    // Escape steps back one layer
    private void Back()
    {
        if (confirmExitPanel.activeSelf) OnExitCancelled();
        else if (settingsPanel.activeSelf) OnSettingsBack();
        else OnResumePressed();
    }

    private void HideAll()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmExitPanel.SetActive(false);
    }
}