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

    void Start()
    {
        errorLabel.fontStyle = FontStyle.Bold;
        // Start checking for invitations immediately and then every 5 seconds
        InvokeRepeating(nameof(CheckForInvitations), 0f, 5f);

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
        string toUsername = searchInputField.text; // Assuming this is the username to invite
        string fromUsername = PlayerPrefs.GetString("Username", "Guest"); // Your username

        if (toUsername == "")
        {
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
        else
        {
            Debug.Log("Invite response: " + responseText);
            // You can add more logic here to update the UI based on the response
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
            Debug.Log("Invitation received: " + responseText);
            // Handle the invitation, e.g., show a popup or UI element for the user to accept
        }
        else if (string.IsNullOrEmpty(responseText))
        {
            Debug.LogError("Error checking for invitations or no response from server.");
        }
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
        Debug.Log(responseText);
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
