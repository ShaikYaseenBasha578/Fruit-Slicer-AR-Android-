using UnityEngine;
using UnityEngine.SceneManagement;  // Required to load scenes

public class HowToPlayController : MonoBehaviour
{
    // This method should be linked to your home screen button
    public void GoToHomeScreen()
    {
        SceneManager.LoadScene("Homescreen");  // Use the name of your home screen scene
        // Alternatively, use the index: SceneManager.LoadScene(0);  // assuming home screen is scene 0 in Build Settings
    }
}
