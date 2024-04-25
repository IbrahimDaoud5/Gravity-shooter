using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{

    public GameObject LoginPanel, RegisterPanel, LobbyPanel,TestLobby;

    void Start()
    {
        string panelToShow = PlayerPrefs.GetString("PanelToShow", "");

        if (panelToShow == "Lobby")
        {
            ShowLobbyPanel();
        }
        PlayerPrefs.DeleteKey("PanelToShow");
    }


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

    public void ShowLobbyPanel()
    {
        ClearInputFields(LobbyPanel);
        LoginPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        TestLobby.SetActive(true);
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
