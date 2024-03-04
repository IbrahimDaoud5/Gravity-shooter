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
    Color darkGreen = new(0f, 0.5f, 0f);
    Color purple = new(0.6f, 0f, 0.6f);
    private List<string> alreadyInvited = new List<string>();


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

    }
    void OnEnable()
    {
        string s = "Welcome " + PlayerPrefs.GetString("Username", "Guest");
        welcomeLabel.color = Color.green;
        welcomeLabel.text = s;//SHOW IN LABEL 

        errorLabel.text = "";

        //Put the current player in the invitation ScrollView
        AddEntryToScrollView(PlayerPrefs.GetString("Username", "Guest") + " (me)", "Not Ready", Color.red);

        // Start checking for invitations immediately and then every 3 seconds
        InvokeRepeating(nameof(CheckForInvitations), 0f, 3f);

    }
    void OnDisable()
    {
        CancelInvoke(nameof(CheckForInvitations));
        CancelInvoke(nameof(CheckInvitationsStatus));

        if (scrollViewContent != null)
        {
            // Iterate through all children of the scrollViewContent and destroy them
            foreach (Transform child in scrollViewContent)
            {
                Destroy(child.gameObject);
            }
        }
    }


    public void CallReady()
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
        errorLabel.color = Color.red;
        string toUsername = searchInputField.text;
        string fromUsername = PlayerPrefs.GetString("Username", "Guest"); // Your username

        if (toUsername == "")
        {
            errorLabel.text = "Enter username first !";
            return;
        }
        // Check if the user has already been invited
        else if (alreadyInvited.Contains(toUsername))
        {
            errorLabel.text = $"Invitation already sent to {toUsername}.";
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
            // Add the user to the alreadyInvited list if its not there
            string usernameToAdd = searchInputField.text;

            if (IsUserStatus(usernameToAdd, "Denied Invitation"))
                DeleteEntryFromScrollView(usernameToAdd);
            alreadyInvited.Add(usernameToAdd);
            AddEntryToScrollView(usernameToAdd, "Invited", purple);


            // Start invoking the CheckInvitationsStatus method every 2 seconds
            InvokeRepeating(nameof(CheckInvitationsStatus), 0f, 2f);
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


            // Find and setup the Accept button
            Button acceptButton = popupInstance.transform.Find("AcceptBtn").GetComponent<Button>();
            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveAllListeners();
                acceptButton.onClick.AddListener(() => UpdateInvitationStatus(popupInstance, "accepted"));
                // Go To MULTI GAME (when you accept the invitation)
            }

            // Setup the Deny button similarly
            Button denyButton = popupInstance.transform.Find("DenyBtn").GetComponent<Button>();
            if (denyButton != null)
            {
                denyButton.onClick.RemoveAllListeners();
                denyButton.onClick.AddListener(() => UpdateInvitationStatus(popupInstance, "denied"));
            }

        }
        else if (string.IsNullOrEmpty(responseText))
        {
            Debug.LogError("Error checking for invitations or no response from server.");
        }
    }
    [Serializable]
    public class InvitationStatusData
    {
        public string fromUsername;
        public string toUsername;
        public string status;
    }
    private void UpdateInvitationStatus(GameObject popupInstance, string status)
    {
        // Construct the data to send, including the action
        var invitationStatusData = new InvitationStatusData
        {
            fromUsername = PlayerPrefs.GetString("Username", "Guest"),
            toUsername = searchInputField.text, // Assuming this is the invited user's username
            status = status // "accepted" or "denied"
        };
        string jsonData = JsonUtility.ToJson(invitationStatusData);

        ServerRequestHandler.Instance.SendRequest("/lobby/updateInvitationStatus", jsonData, HandleInvitationStatusResponse);

        // Close the popup regardless of the action
        ClosePopup(popupInstance);
    }
    private void HandleInvitationStatusResponse(string responseText)
    {
        // Handle the server's response to the invitation status update
        Debug.Log("Invitation status updated: " + responseText);
    }

    void ClosePopup(GameObject popupInstance)
    {
        if (popupInstance != null)
        {
            activePopups.Remove(popupInstance);
            Destroy(popupInstance);
        }
    }




    public void AddEntryToScrollView(string username, string status, Color colorOfStatus)
    {
        if (entryPrefab == null || scrollViewContent == null) return;

        // Instantiate the prefab as a child of the scrollViewContent
        entryPrefab.SetActive(true);
        GameObject newEntry = Instantiate(entryPrefab, scrollViewContent);
        entryPrefab.SetActive(false);
        // Find and set the Username and Status text components
        TMP_Text[] texts = newEntry.GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text text in texts)
        {
            if (text.gameObject.name == "UsernameTxt") // Assuming the GameObject name is "Username"
            {
                text.text = username;
            }
            else if (text.gameObject.name == "StatusTxt") // Assuming the GameObject name is "Status"
            {
                text.color = colorOfStatus;
                text.text = status;
            }
        }
    }


    public void UpdateUserStatus(string username, string newStatus, Color statusColor)
    {
        // Check if the scrollViewContent has any children
        if (scrollViewContent.childCount > 0)
        {
            // Iterate through all children of the scrollViewContent
            for (int i = 0; i < scrollViewContent.childCount; i++)
            {

                // Get the child GameObject
                GameObject child = scrollViewContent.GetChild(i).gameObject;
                // Find the UsernameTxt and StatusTxt within the child GameObject
                TMP_Text usernameText = child.transform.Find("UsernameTxt").GetComponent<TMP_Text>();
                TMP_Text statusText = child.transform.Find("StatusTxt").GetComponent<TMP_Text>();

                // Check if the current child is the one with the matching username
                if (usernameText != null && usernameText.text == username)
                {
                    // If found, update the status text
                    if (statusText != null)
                    {
                        statusText.text = newStatus;
                        statusText.color = statusColor;
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No entries in the ScrollView to search through.");
        }
    }


    public void PlaySolo()
    {
        sceneLoader.LoadScene();
        // Here we need to send to the server to update the player status to: inGame
    }

    public void CallLogout(string playerId)
    {
        LoginData lobbyData = new LoginData(PlayerPrefs.GetString("Username", "Guest"), "");
        string jsonData = JsonUtility.ToJson(lobbyData);
        ServerRequestHandler.Instance.SendRequest("/lobby/logout", jsonData, HandleResponse);
    }

    private void HandleResponse(string responseText)
    { // for ready and for logout
        if (responseText == null)
        {
            Debug.LogError("responseText is Null");
            return;
        }
        else if (responseText == "logged out successfully")
        {
            UImanager.ShowLoginPanel();
        }
        else if (responseText == (PlayerPrefs.GetString("Username", "Guest") + "'s status is set to READY"))
        {
            UpdateUserStatus((PlayerPrefs.GetString("Username", "Guest") + " (me)"), "Ready", darkGreen);
        }

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







    private void CheckInvitationsStatus()
    {
        // foreach (var username in alreadyInvited)
        // {
        // Prepare the request data
        LoginData lobbyData = new LoginData(searchInputField.text, "");
        string jsonData = JsonUtility.ToJson(lobbyData);

        // Send the request to check the invitation status
        ServerRequestHandler.Instance.SendRequest("/lobby/checkInvitationStatus", jsonData, HandleCheckInvitationsStatus);
        //  }
    }

    private void HandleCheckInvitationsStatus(string response)
    {
        if (response == "denied")
        {
            // Remove from the invitation list
            alreadyInvited.Remove(searchInputField.text);
            // Update the status in the scrollbar
            UpdateUserStatus(searchInputField.text, "Denied Invitation", Color.red);
        }
        else if (response == "accepted")
        {
            UpdateUserStatus(searchInputField.text, "Accepted Invitation", Color.blue);

            // GO TO MULTI GAME (if you got accept from the invited user)
        }


    }
    public bool IsUserStatus(string username, string statusToCheck)
    {
        // Check if the scrollViewContent has any children
        if (scrollViewContent.childCount > 0)
        {
            // Iterate through all children of the scrollViewContent
            for (int i = 0; i < scrollViewContent.childCount; i++)
            {
                // Get the child GameObject
                GameObject child = scrollViewContent.GetChild(i).gameObject;
                // Find the UsernameTxt and StatusTxt within the child GameObject
                TMP_Text usernameText = child.transform.Find("UsernameTxt").GetComponent<TMP_Text>();
                TMP_Text statusText = child.transform.Find("StatusTxt").GetComponent<TMP_Text>();

                // Check if the current child is the one with the matching username
                if (usernameText != null && usernameText.text.Equals(username))
                {
                    // Check if the status text is equal to the given statusToCheck
                    if (statusText != null && statusText.text.Equals(statusToCheck, StringComparison.OrdinalIgnoreCase))
                    {
                        return true; // The user has the specified status
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("No entries in the ScrollView to search through.");
        }

        return false; // The user was not found or does not have the specified status
    }

    public void DeleteEntryFromScrollView(string username)
    {
        if (scrollViewContent == null) return;

        // Iterate through all children of the scrollViewContent
        for (int i = 0; i < scrollViewContent.childCount; i++)
        {
            GameObject child = scrollViewContent.GetChild(i).gameObject;
            TMP_Text usernameText = child.transform.Find("UsernameTxt").GetComponent<TMP_Text>();

            // Check if the current child is the one with the matching username
            if (usernameText != null && usernameText.text.Equals(username))
            {
                Destroy(child); // Delete the panel
                return; // Exit the function as the panel has been found and deleted
            }
        }

        Debug.LogWarning($"No entry found for username: {username}");
    }


}
