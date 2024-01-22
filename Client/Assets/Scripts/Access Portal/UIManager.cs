using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject LoginPanel, RegisterPanel, MainmenuPanel;


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
        LoginPanel.SetActive(true);
    }
    public void ShowLoginPanelAfterLogout()//Panel not canvas
    {
        ClearInputFields(LoginPanel);
        MainmenuPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }
    public void ShowMainmenuPanel()
    {
        ClearInputFields(RegisterPanel);
        LoginPanel.SetActive(false);
        MainmenuPanel.SetActive(true);
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
