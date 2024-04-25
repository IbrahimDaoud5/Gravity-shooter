using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private Button joinButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;

    public string lobbyID; 

    void Awake()
    {
        joinButton.onClick.AddListener(JoinLobby);
    }

    public void Setup(string name, string id)
    {
        lobbyNameText.text = name;
        lobbyID = id; 

        Debug.Log($"Setup called with Name: {name}, ID: {id}");
    } 
    public void SetupMyLobby(string name)
    {
        lobbyNameText.text = name;
        Debug.Log($"Setup called with Name: {name}");
    }

    public string GetLobbyName()
    {
        return lobbyNameText.text;
    }

    private void JoinLobby()
    {
        if (string.IsNullOrWhiteSpace(lobbyID))
        {
            return;
           // Debug.LogError("Lobby ID is missing or invalid.");
        }
        Debug.Log($"Joining lobby with ID: {lobbyID}");
        FindObjectOfType<GameLobbyController>().JoinLobbyByID(lobbyID);
    }

    void OnDestroy()
    {
        // Clean up to avoid memory leaks
        joinButton.onClick.RemoveListener(JoinLobby);
    }
}
