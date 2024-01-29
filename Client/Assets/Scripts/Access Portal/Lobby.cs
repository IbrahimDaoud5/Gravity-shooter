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

    void Start()
    {


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
        // Debouncing should be implemented here to optimize server requests
        CallSearchUsers(searchText);
    }

    public void CallSearchUsers(string query)
    {

        string jsonData = JsonUtility.ToJson(searchInputField.text);
        ServerRequestHandler.Instance.SendRequest("/lobby/showConnectedUsers", jsonData, HandleSearchResponse);
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
    /*
    private void UpdateDropdown(string[] users)
    {
        // userDropdown.ClearOptions();

        List<string> options = new List<string>(users);
        // userDropdown.AddOptions(options);
    }
    */

}
