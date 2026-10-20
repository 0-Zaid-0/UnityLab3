using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Score { get; private set; }
    public int Lives { get; private set; } = 3;
    public bool IsGameOver { get; private set; }

    public event System.Action<int> OnScoreChanged;
    public event System.Action<int> OnLivesChanged;
    public event System.Action OnGameOver;

    [SerializeField] int startingLives = 3;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Lives = startingLives;
    }

    void Start()
    {
        OnScoreChanged?.Invoke(Score);
        OnLivesChanged?.Invoke(Lives);
    }

    public void AddScore(int amount)
    {
        if (IsGameOver) return;
        Score += amount;
        OnScoreChanged?.Invoke(Score);
    }

    public void LoseLife()
    {
        if (IsGameOver) return;

        Lives--;
        OnLivesChanged?.Invoke(Lives);

        if (Lives <= 0)
        {
            IsGameOver = true;
            OnGameOver?.Invoke();
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
