using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class IncomeLevel
{
    public int gain = 25;          // Ecto per tick
    public float interval = 6f;    // seconds per tick
    public int upgradeCost = 0;    // cost to reach this level (ignored for level 0)
}

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [Header("Path")]
    public Transform startPoint;
    public Transform endPoint;
    public Transform[] path;

    [Header("Ecto")]
    public int ecto;
    [SerializeField] private int startingEcto = 125;

    [Header("Passive Income (placeholder numbers, tune these)")]
    [SerializeField] private List<IncomeLevel> incomeLevels = new List<IncomeLevel>
    {
        new IncomeLevel { gain = 25, interval = 7f, upgradeCost = 0 },
        new IncomeLevel { gain = 25, interval = 6f, upgradeCost = 75 },
        new IncomeLevel { gain = 25, interval = 5f, upgradeCost = 150 },
        new IncomeLevel { gain = 25, interval = 4f, upgradeCost = 300 },
        new IncomeLevel { gain = 25, interval = 3f, upgradeCost = 500 },
    };

    public UnityEvent onIncomeChanged = new UnityEvent();

    private int incomeIndex;
    private float incomeProgress; // 0 to 1

    public int IncomeLevelIndex => incomeIndex;
    public IncomeLevel CurrentIncome => incomeLevels[incomeIndex];
    public IncomeLevel NextIncome => incomeIndex + 1 < incomeLevels.Count ? incomeLevels[incomeIndex + 1] : null;
    public float IncomePerSecond => CurrentIncome.gain / CurrentIncome.interval;
    public float IncomeProgress => incomeProgress;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        ecto = startingEcto;
    }

    private void Update()
    {
        incomeProgress += Time.deltaTime / CurrentIncome.interval;

        if (incomeProgress >= 1f)
        {
            incomeProgress -= 1f;
            IncreaseEcto(CurrentIncome.gain);
        }
    }

    public bool TryUpgradeIncome()
    {
        IncomeLevel next = NextIncome;
        if (next == null) return false;
        if (!SpendEcto(next.upgradeCost)) return false;

        incomeIndex++;
        onIncomeChanged.Invoke();
        return true;
    }

    public void IncreaseEcto(int amount)
    {
        ecto += amount;
    }

    public bool SpendEcto(int amount)
    {
        if (amount <= ecto)
        {
            ecto -= amount;
            return true;
        }

        Debug.Log("Insufficient Ecto!");
        return false;
    }
}