using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject LoginPanel, RegisterPanel;


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


    public void ClearInputFields(GameObject panel)
    {
        var inputFields = panel.GetComponentsInChildren<TMP_InputField>(true);
        foreach (var inputField in inputFields)
        {
            inputField.text = "";
        }
    }

}
