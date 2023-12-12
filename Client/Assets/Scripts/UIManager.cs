using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject LoginPanel, RegisterPanel;


    public void ShowRegisterCanvas()
    {
        LoginPanel.SetActive(false);
        RegisterPanel.SetActive(true);


        if (RegisterPanel.activeSelf && !LoginPanel.activeSelf)
            Debug.Log(" \n\n### Transitioning to register screen ### ");
    }

}
