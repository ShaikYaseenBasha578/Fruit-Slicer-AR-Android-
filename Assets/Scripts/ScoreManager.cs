using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;        // Reference to the UI Text to display the score
    public Text highScoreText;    // Reference to the UI Text to display the high score

    private int currentScore = 0; // Player's current score
    private int highScore = 0;    // Player's high score

    private void Start()
    {
        // Load the high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;
    }

    // Call this method when the player scores points
    public void AddScore(int points)
    {
        currentScore += points;
        scoreText.text = "Score: " + currentScore;

        // Update high score if the current score exceeds the high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            highScoreText.text = "High Score: " + highScore;

            // Save the new high score
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    // Call this method to reset the score (e.g., when restarting the game)
    public void ResetScore()
    {
        currentScore = 0;
        scoreText.text = "Score: " + currentScore;
    }
}
