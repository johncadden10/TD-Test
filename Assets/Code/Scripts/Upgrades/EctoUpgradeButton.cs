using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EctoUpgradeButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image tickFill; // optional: Filled Image showing progress to the next tick

    private void Start()
    {
        button.onClick.AddListener(() => LevelManager.main.TryUpgradeIncome());
    }

    private void Update()
    {
        LevelManager lm = LevelManager.main;
        IncomeLevel next = lm.NextIncome;

        if (tickFill != null) tickFill.fillAmount = lm.IncomeProgress;

        if (next == null)
        {
            label.text = $"Income MAX ({lm.IncomePerSecond:0.#}/s)";
            costText.text = "";
            button.interactable = false;
            return;
        }

        float nextRate = next.gain / next.interval;
        label.text = $"Income {lm.IncomePerSecond:0.#}/s > {nextRate:0.#}/s";
        costText.text = next.upgradeCost.ToString();
        button.interactable = lm.ecto >= next.upgradeCost;
    }
}