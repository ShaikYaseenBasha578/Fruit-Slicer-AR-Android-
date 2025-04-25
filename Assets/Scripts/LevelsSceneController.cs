using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsSceneController : MonoBehaviour
{
    // Navigate to the Levels Selection scene
    public void GoToLevelsScene()
    {
        SceneManager.LoadScene("Levels"); // Ensure "Levels" matches the name in your Build Settings
    }
}
