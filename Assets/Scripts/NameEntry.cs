using Csluder2.FusionWork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class NameEntry : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] Button submitButton;
    [SerializeField] GameObject TextBox;

    [SerializeField] GameObject SubmitBox;
    [SerializeField] GameObject ScrollView;

    public void SubmitName()
    {
        FusionConnection.instance.ConnectToLobby(nameInputField.text);
        TextBox.SetActive(false);
        SubmitBox.SetActive(false);
        ScrollView.SetActive(true);
    }

    public void ActivateButton()
    {
        submitButton.interactable = true;
    }

}
