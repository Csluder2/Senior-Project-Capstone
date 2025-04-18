using UnityEngine;
using UnityEngine.SceneManagement; // For scene management

public class MainMenuManager : MonoBehaviour
{
    // Called when the mouse hovers over the button
    public void OnHoverEnter(GameObject button)
    {
        // Example: Change the button color on hover
        var buttonImage = button.GetComponent<UnityEngine.UI.Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.red; // Change to yellow on hover
        }
    }

    // Called when the mouse stops hovering over the button
    public void OnHoverExit(GameObject button)
    {
        // Reset the button color when not hovered
        var buttonImage = button.GetComponent<UnityEngine.UI.Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.white; // Reset to white
        }
    }

    // Play Button: Loads the CharacterSelect scene
    public void OnPlayButtonClick()
    {
        SceneManager.LoadScene("OfflineOnline");
    }

    // Settings Button: Loads the Settings scene
    public void OnSettingsButtonClick()
    {
        SceneManager.LoadScene("Settings");
    }
    // Instruction Button: Loads Instruction scene
    public void OnInstructionsButtonClick()
    {
        SceneManager.LoadScene("Instructions");
    }

    // Quit Button: Exits the game
    public void OnQuitButtonClick()
    {
        Application.Quit();
        Debug.Log("Game is exiting..."); // Debug log for testing in the editor
    }
}

