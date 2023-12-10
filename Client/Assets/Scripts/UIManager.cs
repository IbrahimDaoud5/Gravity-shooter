using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject LoginCanvas;
    public GameObject RegisterCanvas;

    public void ShowRegisterCanvas()
    {
        LoginCanvas.SetActive(false);
        RegisterCanvas.SetActive(true);
    }

}
