using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    public Text popupText; // Assign this in the inspector
    public GameObject popupPanel; // Optional, assign if using a panel

    private void Start()
    {
        HidePopup(); // Start with the popup hidden
    }

    public void ShowPopup(string message, float duration)
    {
        popupText.text = message;
        popupPanel.SetActive(true); // Enable the panel
        popupText.gameObject.SetActive(true); // Enable the text
        StartCoroutine(HidePopupAfterTime(duration));
    }

    private IEnumerator HidePopupAfterTime(float duration)
    {
        yield return new WaitForSeconds(duration);
        HidePopup();
    }

    private void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
        popupText.gameObject.SetActive(false);
    }
}
