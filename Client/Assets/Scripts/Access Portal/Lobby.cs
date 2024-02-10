using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public UIManager UImanager;
    public Text welcomeLabel;
    public TMP_InputField searchInputField;
    public SearchableDropDown searchableDropdown;
    public Text errorLabel;
    public GameObject invitationPopupPrefab;
    public Transform lobbyPanelTransform;
    public TMP_Text messageText;
    private List<GameObject> activePopups = new List<GameObject>();
    public Transform scrollViewContent;
    public GameObject entryPrefab;
    public SceneLoader sceneLoader;


    void Start()
    {
        errorLabel.fontStyle = FontStyle.Bold;

        if (searchInputField != null)
        {
            searchInputField.onSelect.AddListener(OnSearchChanged);
        }
    }
    void Update()
    {
        string s = "Welcome " + PlayerPrefs.GetString("Username", "Guest");
        welcomeLabel.color = Color.green;
        welcomeLabel.text = s;//SHOW IN LABEL
    }
    void OnEnable()
    {
        errorLabel.text = "";

        // Start checking for invitations immediately and then every 3 seconds
        InvokeRepeating(nameof(CheckForInvitations), 0f, 3f);
    }
    void OnDisable()
    {
        CancelInvoke(nameof(CheckForInvitations));
    }


    public void CallLobby(string playerId)
    {
        if (ServerRequestHandler.Instance == null)
        {
            Debug.LogError("ServerRequestHandler instance is null.");
            return;
        }
        LoginData lobbyData = new LoginData(PlayerPrefs.GetString("Username", "Guest"), "");
        string jsonData = JsonUtility.ToJson(lobbyData);
        ServerRequestHandler.Instance.SendRequest("/lobby/setReady", jsonData, HandleResponse);
    }

    private void OnSearchChanged(string searchText)
    {

        CallSearchUsers(searchText);
        errorLabel.text = "";
    }

    public void CallSearchUsers(string query)
    {
        string jsonData = JsonUtility.ToJson(searchInputField.text);
        ServerRequestHandler.Instance.SendRequest("/lobby/showConnectedUsers", jsonData, HandleSearchResponse);
    }

    [Serializable]
    public class InviteData
    {
        public string fromUsername;
        public string toUsername;
    }

    public void CallInvite()
    {
        string toUsername = searchInputField.text;
        string fromUsername = PlayerPrefs.GetString("Username", "Guest"); // Your username

        if (toUsername == "")
        {
            errorLabel.color = Color.red;
            errorLabel.text = "Enter username first !";
            return;
        }

        var inviteData = new InviteData
        {
            fromUsername = fromUsername,
            toUsername = toUsername
        };
        string jsonData = JsonUtility.ToJson(inviteData);

        //JUST FOR TESTING
        Debug.Log(toUsername);
        Debug.Log(jsonData);
        //JUST FOR TESTING

        // Use ServerRequestHandler to send the invite
        ServerRequestHandler.Instance.SendRequest("/lobby/sendInvite", jsonData, HandleInviteResponse);
    }

    // Callback method to handle the response from sending an invite
    private void HandleInviteResponse(string responseText)
    {
        if (string.IsNullOrEmpty(responseText))
        {
            Debug.LogError("Failed to send invite or no response from server.");
        }
        else if (responseText == "User is not available")
        {
            Debug.Log(responseText);
            errorLabel.color = Color.red;
            errorLabel.text = responseText;
        }
        else
        {
            Debug.Log("Invite response: " + responseText);
            errorLabel.color = Color.green;
            errorLabel.text = responseText;
            AddEntryToScrollView("abc787");

        }
    }


    public void CheckForInvitations()
    {

        LoginData userData = new LoginData(PlayerPrefs.GetString("Username", "Guest"), "");
        string jsonData = JsonUtility.ToJson(userData);

        ServerRequestHandler.Instance.SendRequest("/lobby/checkInvite", jsonData, HandleCheckInvitationsResponse);
    }


    // Callback method to handle the response from checking for invitations
    private void HandleCheckInvitationsResponse(string responseText)
    {
        if (!string.IsNullOrEmpty(responseText) && responseText != "No invitations")
        {
            // Set the invitation message
            messageText.text = responseText;
            // Instantiate the popup and set it as a child of the lobby panel
            GameObject popupInstance = Instantiate(invitationPopupPrefab);
            popupInstance.SetActive(true); // Make sure the popup is visible
            popupInstance.transform.SetParent(lobbyPanelTransform, false); // Ensure it scales correctly

            // Define both vertical and horizontal offsets
            float verticalOffset = 30;   // The vertical offset between popups
            float horizontalOffset = 30; // The horizontal offset between popups

            // Calculate the new position based on the number of active popups
            Vector3 newPosition = popupInstance.transform.localPosition;
            newPosition.y -= verticalOffset * activePopups.Count; // Move each new popup down
            newPosition.x += horizontalOffset * activePopups.Count; // Move each new popup to the right

            popupInstance.transform.localPosition = newPosition;
            // Add popup to the list of active popups
            activePopups.Add(popupInstance);

            // Find the Deny button and attach the ClosePopup action
            Button denyButton = popupInstance.transform.Find("DenyBtn").GetComponent<Button>();

            if (denyButton != null)
            {
                denyButton.onClick.RemoveAllListeners(); // Clear existing listeners to prevent duplication
                denyButton.onClick.AddListener(() => ClosePopup(popupInstance));
            }
        }
        else if (string.IsNullOrEmpty(responseText))
        {
            Debug.LogError("Error checking for invitations or no response from server.");
        }
    }

    void ClosePopup(GameObject popupInstance)
    {
        if (popupInstance != null)
        {
            activePopups.Remove(popupInstance);
            Destroy(popupInstance);
        }
    }





    public void AddEntryToScrollView(string text)
    {
        if (scrollViewContent == null || entryPrefab == null) return;

        GameObject newEntry = Instantiate(entryPrefab, scrollViewContent);
        TMP_Text textComponent = newEntry.GetComponent<TMP_Text>();
        if (textComponent != null)
            textComponent.text = text;
    }


    public void PlaySolo()
    {
        sceneLoader.LoadScene();
    }









    public void CallLogout(string playerId)
    {
        LoginData lobbyData = new LoginData(PlayerPrefs.GetString("Username", "Guest"), "");
        string jsonData = JsonUtility.ToJson(lobbyData);
        ServerRequestHandler.Instance.SendRequest("/lobby/logout", jsonData, HandleResponse);
    }

    private void HandleResponse(string responseText)
    {
        if (responseText == null)
        {
            // Handle error
            return;
        }
        else if (responseText == "logged out successfully")
        {
            UImanager.ShowLoginPanel();
        }

        // Process the response
        // Additional response handling logic...
    }


    private void HandleSearchResponse(string responseText)
    {
        if (responseText == null)
        {
            // Handle error
            Debug.LogError("Error in server response");
            return;
        }
        string[] users = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(responseText);
        users = users.Where(user => user != PlayerPrefs.GetString("Username", "Guest")).ToArray();
        UpdateDropdown(users);
    }

    private void UpdateDropdown(string[] users)
    {
        // Assuming that 'searchableDropdown' is assigned in the Inspector.
        if (searchableDropdown != null)
        {
            searchableDropdown.ResetDropDown();
            searchableDropdown.ClearOptions();
            searchableDropdown.AddItemToScrollRect(new List<string>(users));
        }
    }
}
