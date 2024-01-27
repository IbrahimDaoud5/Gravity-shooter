using UnityEngine;
using UnityEngine.UI;
public class Lobby : MonoBehaviour
{
    public UIManager UImanager;
    public Text welcomeLabel;

    void Start()
    {

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
}
