using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    public Transform startPoint;
    public Transform endPoint;
    public Transform[] path;

    public int ecto;

    private int ectoTimerGain = 25;
    private float ectoTimer = 6f;
    private float timeRemaining;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        ecto = 125;
        timeRemaining = ectoTimer;
    }

    private void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            IncreaseEcto(ectoTimerGain);
            timeRemaining = ectoTimer;
        }
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
        else
        {
            Debug.Log("Insufficient Ecto!");
            return false;
        }
    }
}
