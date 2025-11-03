using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel; // панель или текст
    [SerializeField] private GameObject restartButton; // кнопка в Canvas

    public bool IsGameOver { get; private set; }
    private int score;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        score = 0;
        UpdateScoreUI();

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (restartButton) restartButton.SetActive(false);
    }

    public void AddScore(int v)
    {
        score += v;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = $"Score: {score}";
    }

    public void GameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (restartButton) restartButton.SetActive(true);
    }

    public void Restart()
    {
        Scene s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.name);
    }
}