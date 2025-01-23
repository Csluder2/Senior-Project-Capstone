using UnityEngine;

using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    // This method will be called when the button is clicked
    public void OnBackButtonClick()
    {
        // Load the previous scene or the main menu
        // For example, loading the main menu:
        SceneManager.LoadScene("MainMenu");
    }
}
