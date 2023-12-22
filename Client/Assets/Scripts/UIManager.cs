using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject LoginPanel, RegisterPanel;


    public void ShowRegisterPanel()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);
    }
    public void ShowLoginPanel()//Panel not canvas
    {
        RegisterPanel.SetActive(false);
        LoginPanel.SetActive(true);
    }

}
