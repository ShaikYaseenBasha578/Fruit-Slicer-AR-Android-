using UnityEngine;
using UnityEngine.SceneManagement; // Required to load scenes

public class SceneManagerScript : MonoBehaviour
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
}
