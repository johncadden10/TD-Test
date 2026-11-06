using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject gameOverPanel;

    private void Start()
    {
        gameOverPanel.SetActive(false);
        LivesManager.onGameOver.AddListener(ShowGameOver);
    }

    private void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        GameSpeed.Pause();
    }

    public void OnRetryPressed()
    {
        GameSpeed.ClearPauses();   // keeps your chosen speed, only removes the pause
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}