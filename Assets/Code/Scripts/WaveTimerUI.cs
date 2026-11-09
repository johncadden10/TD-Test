using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button callEarlyButton; // optional

    private void Start()
    {
        if (callEarlyButton != null)
            callEarlyButton.onClick.AddListener(() => EnemySpawner.Instance.CallNextWave());
    }

    private void Update()
    {
        EnemySpawner s = EnemySpawner.Instance;
        if (s == null) return;

        waveText.text = $"Wave {s.WavesLaunched}/{s.TotalWaves}";

        if (s.AllWavesLaunched)
        {
            timerText.text = "Final wave";
            if (callEarlyButton != null) callEarlyButton.interactable = false;
        }
        else
        {
            timerText.text = $"Next wave: {Mathf.CeilToInt(s.Countdown)}s";
        }
    }
}