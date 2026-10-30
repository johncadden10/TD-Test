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
        Time.timeScale = 0f; // pause the game
    }

    public void OnRetryPressed()
    {
        Time.timeScale = 1f; // restore time before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}