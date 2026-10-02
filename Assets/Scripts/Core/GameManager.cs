using UnityEngine;
using UnityEngine.SceneManagement;

// Стежить за кінцем гри: зомбі дійшов до будинку -> Game Over, усі зомбі мертві -> Win.
public class GameManager : MonoBehaviour
{
    [SerializeField] private GridManager grid;
    [SerializeField] private ZombieSpawner spawner;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;
    [Tooltip("Наскільки лівіше від газону зомбі має зайти, щоб гра програлась.")]
    [SerializeField] private float loseMargin = 0.3f;

    public static bool IsGameOver { get; private set; }

    private void Awake()
    {
        IsGameOver = false;
        Time.timeScale = 1f;

        if (winPanel != null) winPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (IsGameOver)
            return;

        float loseX = grid.transform.position.x - loseMargin;
        foreach (Zombie zombie in Zombie.Active)
        {
            if (zombie.transform.position.x < loseX)
            {
                EndGame(gameOverPanel);
                return;
            }
        }

        if (spawner.IsFinished && Zombie.Active.Count == 0)
            EndGame(winPanel);
    }

    private void EndGame(GameObject panel)
    {
        IsGameOver = true;
        Time.timeScale = 0f;
        if (panel != null)
            panel.SetActive(true);
    }

    // Признач на кнопку Restart (On Click).
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
