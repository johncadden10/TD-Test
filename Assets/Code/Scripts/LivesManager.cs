using UnityEngine;
using UnityEngine.Events;

public class LivesManager : MonoBehaviour
{
    public static LivesManager main;

    [Header("Attributes")]
    [SerializeField] private int startingLives = 20;

    public static UnityEvent onLivesChanged = new UnityEvent();
    public static UnityEvent onGameOver = new UnityEvent();

    public int CurrentLives { get; private set; }

    private void Awake()
    {
        main = this;
        CurrentLives = startingLives;
    }

    public void LoseLife(int amount = 1)
    {
        CurrentLives = Mathf.Max(0, CurrentLives - amount);
        onLivesChanged.Invoke();

        if (CurrentLives <= 0)
        {
            onGameOver.Invoke();
            Debug.Log("Game Over!");
        }
    }
}
