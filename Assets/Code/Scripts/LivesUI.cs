using UnityEngine;
using TMPro;

public class LivesUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI livesText;

    private void Start()
    {
        LivesManager.onLivesChanged.AddListener(UpdateUI);
        LivesManager.onGameOver.AddListener(ShowGameOver);
        UpdateUI();
    }

    private void UpdateUI()
    {
        livesText.text = $"{LivesManager.main.CurrentLives}";
    }

    private void ShowGameOver()
    {
        livesText.text = "0";
    }
}
