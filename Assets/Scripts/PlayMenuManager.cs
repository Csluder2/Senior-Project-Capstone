using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenuManager : MonoBehaviour
{
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


    public void OnOfflineButtonClick()
    {
        SceneManager.LoadScene("FightingStage");
    }

    // Settings Button: Loads the Settings scene
    public void OnOnlineButtonClick()
    {
        SceneManager.LoadScene("OnlineMenu");
    }

}
