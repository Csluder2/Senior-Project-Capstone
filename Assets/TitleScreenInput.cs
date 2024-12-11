using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    public string nextSceneName = "MainMenu"; // Replace with your next scene's name.

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Detect the Enter key.
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}

