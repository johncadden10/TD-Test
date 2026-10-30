using UnityEngine;

public class Reaper : Ghost
{
    [Header("Reaper References")]
    [SerializeField] private ReaperText reaperText;

    [Header("Reaper Attributes")]
    [SerializeField] private int ectoGain = 20;
    [SerializeField] private float generationSpeed = 7.5f;

    private float timeRemaining;
    private bool isGenerating = false;

    protected override void Start()
    {
        base.Start();
        timeRemaining = generationSpeed;
    }

    protected override void Update()
    {
        if (isGenerating) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            if (towerAnimator != null)
            {
                isGenerating = true;
                towerAnimator.SetGenerating();
            }
            else
            {
                OnGenerateEcto();
            }
            timeRemaining = generationSpeed;
        }
    }

    public void OnGenerateEcto()
    {
        LevelManager.main.IncreaseEcto(ectoGain);
        if (reaperText != null)
            reaperText.TriggerEffect();
    }

    public void OnGenerateEnd()
    {
        isGenerating = false;
        timeRemaining = generationSpeed;
    }

    protected override void Shoot() { }
}