using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject LoginPanel, RegisterPanel, LobbyPanel;


    public void ShowRegisterPanel()
    {
        ClearInputFields(RegisterPanel);
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }
    public void ShowLoginPanel()//Panel not canvas
    {
        ClearInputFields(LoginPanel);
        RegisterPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }
    /*
    public void ShowLoginPanelAfterLogout()//Panel not canvas
    {
        ClearInputFields(LoginPanel);
        LobbyPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }
    */
    public void ShowLobbyPanel()
    {
        ClearInputFields(LobbyPanel);
        LoginPanel.SetActive(false);
        LobbyPanel.SetActive(true);
    }


    public void ClearInputFields(GameObject panel)
    {
        var inputFields = panel.GetComponentsInChildren<TMP_InputField>(true);
        foreach (var inputField in inputFields)
        {
            inputField.text = "";
        }
    }

}
