using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Blade blade;
    [SerializeField] private BaseSpawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText; // Text for displaying high score
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject restartButton; // Restart button
    [SerializeField] private GameObject nextLevelButton; // Next level button
    [SerializeField] private Text gameOverText; // Game Over text

    public int score { get; private set; } = 0;
    private float highScore;

    private bool isEndlessMode = false; // Flag for level 3
    private int currentSceneIndex;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Load the high score when the game starts
        highScore = PlayerPrefs.GetFloat("hiscore", 0);
        UpdateHighScoreText();
        NewGame();
    }

    private void NewGame()
    {
        Time.timeScale = 1f;
        ClearScene();
        fadeImage.color = Color.clear;

        blade.enabled = true;
        spawner.StartSpawning();

        score = 0;
        scoreText.text = score.ToString();

        if (restartButton != null) restartButton.SetActive(false);
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);
        if (nextLevelButton != null) nextLevelButton.SetActive(false);
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();
        foreach (Fruit fruit in fruits)
        {
            if (fruit != null)
            {
                Destroy(fruit.gameObject);
            }
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();
        foreach (Bomb bomb in bombs)
        {
            if (bomb != null)
            {
                Destroy(bomb.gameObject);
            }
        }
    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        // Debug log to check score value
        Debug.Log("Current Score: " + score);

        // Update high score if necessary
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("hiscore", highScore);
            UpdateHighScoreText();
        }

        // Check and display the "Next Level" button when score reaches 20
        if (score >= 10 && !nextLevelButton.activeSelf)
        {
            Debug.Log("Next level button should appear.");
            nextLevelButton.SetActive(true); // Activate the button
        }
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + highScore.ToString();
        }
    }

    public void Explode()
    {
        if (isEndlessMode && IsSpecialObjectCut())
        {
            EndGame();
            return;
        }

        blade.enabled = false;
        spawner.StopSpawning();

        if (gameOverText != null && !gameOverText.gameObject.activeSelf)
            gameOverText.gameObject.SetActive(true);

        if (restartButton != null && !restartButton.activeSelf)
            restartButton.SetActive(true);

        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 0f;
        fadeImage.color = Color.white;
    }

    public void OnRestartButtonClick()
    {
        Time.timeScale = 1f;
        NewGame();

        // Reset spawner after restarting
        if (spawner is SIH_Spawner sihSpawner)
        {
            sihSpawner.ResetSpawner();
        }
    }

    public void OnNextLevelButtonClick()
    {
        if (currentSceneIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }

    private bool IsSpecialObjectCut()
    {
        // Check if a special object was cut
        // Implement your logic here, such as checking tags or a specific condition
        return false; // Replace this with actual logic
    }

    private void EndGame()
    {
        // Handle end-game logic for endless mode
        blade.enabled = false;
        spawner.StopSpawning();

        if (gameOverText != null && !gameOverText.gameObject.activeSelf)
            gameOverText.gameObject.SetActive(true);

        if (restartButton != null && !restartButton.activeSelf)
            restartButton.SetActive(true);

        Time.timeScale = 0f;
    }
}
