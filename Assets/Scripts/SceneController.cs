using UnityEngine;
using UnityEngine.SceneManagement; // Required to load scenes

public class SceneController : MonoBehaviour
{
    // This method loads the game scene
    public void StartGame()
    {
        SceneManager.LoadScene("FruitSlicerAR");
    }

    // This method quits the game
    public void QuitGame()
    {
        Application.Quit(); // Quits the application
#if UNITY_EDITOR
        // If in the editor, simulate quitting the application
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Optional method for loading How to Play or other scenes
    public void LoadHowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    // This method loads the next level based on the current scene
    public void LoadNextLevel()
    {
        // Get the current active scene index and load the next one
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex); // Load the next scene
        }
        else
        {
            Debug.Log("No more levels to load.");
        }
    }
}

