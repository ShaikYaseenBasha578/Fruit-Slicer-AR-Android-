using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject restartButton; // Restart Button reference
    [SerializeField] private Text gameOverText; // Game Over Text reference

    public int score { get; private set; } = 0;

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
        NewGame();
    }

    private void NewGame()
    {
        // Reset time scale to ensure the game is running
        Time.timeScale = 1f;

        // Clear the scene of game objects like fruits and bombs
        ClearScene();

        // **Ensure the fade image is fully reset to transparent**
        fadeImage.color = Color.clear;

        // Re-enable game logic
        blade.enabled = true;
        spawner.enabled = true;

        // Reset the score and update the UI
        score = 0;
        scoreText.text = score.ToString();

        // Ensure restart button and game over text are hidden
        if (restartButton != null) restartButton.SetActive(false);
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
    }

    private void ClearScene()
    {
        // Destroy fruits and bombs, but don't touch UI elements
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

        // Update high score
        float hiscore = PlayerPrefs.GetFloat("hiscore", 0);

        if (score > hiscore)
        {
            hiscore = score;
            PlayerPrefs.SetFloat("hiscore", hiscore);
        }
    }

    public void Explode()
    {
        // Disable game logic
        blade.enabled = false;
        spawner.enabled = false;

        // Show Game Over text and Restart button if they exist
        if (gameOverText != null && !gameOverText.gameObject.activeSelf)
            gameOverText.gameObject.SetActive(true);

        if (restartButton != null && !restartButton.activeSelf)
            restartButton.SetActive(true);

        // Start the fade-out effect
        StartCoroutine(ExplodeSequence());
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        // Fade to white
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Pause the game, and wait for the player to restart
        Time.timeScale = 0f;  // Game is paused, waiting for restart

        // Ensure that when the game is restarted, the fade is reset
        fadeImage.color = Color.white;
    }

    // Called when the player clicks the "Restart" button
    public void OnRestartButtonClick()
    {
        // Resume the game and start a new one
        Time.timeScale = 1f; // Resume the game
        NewGame(); // Reset the game state

        // Ensure that the UI is cleared appropriately after restart
        if (restartButton != null) restartButton.SetActive(false);
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);

        if (restartButton != null)
            Debug.Log("Restart Button exists and is active: " + restartButton.activeSelf);
        else
            Debug.LogError("Restart Button is null!");


    }
}
