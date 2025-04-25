using UnityEngine;
using UnityEngine.SceneManagement;

public class FruitSlicerARSceneManager : MonoBehaviour
{
    // This method navigates to the home screen
    public void GoToHomeScreen()
    {
        SceneManager.LoadScene("Homescreen"); // Use the exact name of your home screen scene
    }

    // Method to load Level 1
    public void LoadLevel1()
    {
        SceneManager.LoadScene("FruitSlicerAR"); // Level 1 scene
    }

    // Method to load Level 2
    public void LoadLevel2()
    {
        SceneManager.LoadScene("FruitSlicerAR1"); // Level 2 scene
    }

    // Method to load Level 3
    public void LoadLevel3()
    {
        SceneManager.LoadScene("FruitSlicerAR2"); // Level 3 scene
    }

    // Reload the current scene (useful for restarting a level)
    public void RestartCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Quit the application (useful for mobile devices)
    public void QuitGame()
    {
        Application.Quit();
    }
}

