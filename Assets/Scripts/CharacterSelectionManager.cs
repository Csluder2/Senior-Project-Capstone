using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionManager : MonoBehaviour
{
    public Button[] characterButtons; // Assign in the inspector
    public TextMeshProUGUI infoText; // Text to show instructions

    private int selectedCount = 0; // Number of selected characters
    private bool[] isSelected; // Track which characters are selected
    private int[] SelectedCharacters = new int[2];
    void Start()
    {
        // Initialize the isSelected array with the same length as the number of buttons
        isSelected = new bool[characterButtons.Length];

        // Assign button click listeners
        for (int i = 0; i < characterButtons.Length; i++)
        {
            int index = i; // Local copy of index for closure
            characterButtons[i].onClick.AddListener(() => OnCharacterSelected(index));
        }

        // Initialize the infoText
        infoText.text = "Select two characters";
    }

    // Handle character selection
    void OnCharacterSelected(int index)
    {
        if (isSelected[index]) // If the character is already selected, deselect it
        {
            isSelected[index] = false;
            characterButtons[index].GetComponent<Image>().color = Color.white; // Reset color
            selectedCount--;
        }
        else // Otherwise, select it
        {
            if (selectedCount < 2) // Ensure no more than two selections
            {
                isSelected[index] = true;
                characterButtons[index].GetComponent<Image>().color = Color.green; // Change color on selection
                selectedCount++;
            }
        }

        // Update the info text once two characters are selected
        if (selectedCount == 2)
        {

            infoText.text = "Press Enter to continue!";



        }
        else
        {
            infoText.text = "Select two characters";
        }
    }
    void Update()
    {
        // Check for Enter key after two characters are selected
        if (selectedCount == 2 && Input.GetKeyDown(KeyCode.Return))
        {
            PlayerPrefs.SetInt("Player1Character", SelectedCharacters[0]);
            PlayerPrefs.SetInt("Player2Character", SelectedCharacters[1]);
            PlayerPrefs.Save();
            // Load the next scene (replace "NextScene" with your actual scene name)
            UnityEngine.SceneManagement.SceneManager.LoadScene("FightingStage");
        }
    }
}

