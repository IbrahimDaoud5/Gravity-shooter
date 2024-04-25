using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;

public class GameLobbyController : NetworkBehaviour
{
    [SerializeField] private Button lobbiesListBtn;
    [SerializeField] private Button createLobbyBtn;
    [SerializeField] private Transform lobbyListParent;
    [SerializeField] private Transform myLobbyList;
    [SerializeField] private GameObject lobbyItemPrefab;
    [SerializeField] private GameObject lobbyPanel;//previousPanel
    [SerializeField] private GameObject lobbiesPanel;
    [SerializeField] private GameObject myLobbyPanel;
    [SerializeField] private Button backButton;
    [SerializeField] private Button startGameButton;
    //---------------------------relay----------------------
    public GameObject targetPrefab;
    private List<Vector3> targetPositions;
    public SpawnPosition spawnPosition;
    private NetworkVariable<string> relayJoinCode = new NetworkVariable<string>();
    //--------------------------------------
    private string lobbyiD;
    private float lobbyUpdateTimer;
    private float heartbeatTimer;

    private Lobby hostLobby;
    private Lobby joinedLobby;


    private async void Start()
    {
        targetPositions = spawnPosition.savedPositions;
        lobbyPanel.SetActive(true);
        lobbiesPanel.SetActive(false);
       /* await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();*/

        createLobbyBtn.onClick.AddListener(async () => await CreateLobby());
        lobbiesListBtn.onClick.AddListener(async () => await ListLobbies());
        startGameButton.onClick.AddListener(() => StartGameAsHost());
        backButton.onClick.AddListener(() => LeaveLobbyAsync());

    }


    private async void LeaveLobbyAsync()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            Debug.Log("Left lobby successfully.");
            StopHeartbeatTimer();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to leave lobby: {e}");
        }
        SwitchToPreviousPanel();
    }

    private void SwitchToPreviousPanel()
    {
        myLobbyPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    //----------------


    private void Update()
    {
        HandleLobbyHeartbeat();
        //HandleLobbyPollUpdates();
        if (hostLobby != null)
        {
            HandleLobbyUpdates(hostLobby);
        }
        if (joinedLobby != null)
        {
            HandleLobbyUpdates(joinedLobby);
        }
    }


    private async Task CreateLobby()
    {
        var username = PlayerPrefs.GetString("Username", "Guest");
        var lobbyOptions = new CreateLobbyOptions
        {

            IsPrivate = false,
            Player = GetPlayer(),
        };
        try
        {
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(username, 4, lobbyOptions);
            lobbyiD = lobby.Id;
            hostLobby = lobby;
            joinedLobby = hostLobby;
            Debug.Log("Lobby created successfully: " + lobbyiD + " lobby code " + lobby.LobbyCode + " lobby name :" + lobby.Name);
            StartHeartbeatTimer();
            printPlayers(hostLobby);
            foreach (Transform child in myLobbyList)
            {
                Destroy(child.gameObject);
            }
            /*GameObject item = Instantiate(lobbyItemPrefab, myLobbyList);
            var itemScript = item.GetComponent<LobbyItem>();
            if (itemScript != null)
            {
                itemScript.SetupMyLobby(username);
            }
            await CreateRelay();
            SwitchToLobbyPanel(); // Switch to the lobby panel*/
            SceneManager.LoadScene("MultiGame");
            await CreateRelay();
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Failed to create lobby: " + e);
        }

    }

    public async void JoinLobbyByID(string lobbyID)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            Debug.Log($"Attempting to join lobby with ID: {lobbyID}");

            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyID, joinLobbyByIdOptions);
            joinedLobby = lobby;
            Debug.Log($"Successfully joined the lobby: {lobbyID}");

            // Retrieve the relay join code from lobby custom data
            string relayCode = lobby.Data["RelayJoinCode"].Value;
           

            StartHeartbeatTimer();
            printPlayers(lobby);
            foreach (Transform child in myLobbyList)
            {
                Destroy(child.gameObject);
            }
            /*GameObject item = Instantiate(lobbyItemPrefab, myLobbyList);
            var itemScript = item.GetComponent<LobbyItem>();
            if (itemScript != null)
            {
                itemScript.SetupMyLobby(username);
            }
            await UpdateLobbyUI(lobby);
            SwitchToLobbyPanel(); // Switch to the lobby panel*/
            SceneManager.LoadScene("MultiGame");
            JoinRelay(relayCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"Failed to join lobby: {e}");
        }
    }


    private Player GetPlayer()
    {
        var username = PlayerPrefs.GetString("Username", "Guest");
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject> {
                    {"PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, username)}
                }
        };
    }

    private async Task UpdateLobbyUI(Lobby lobbyi)
    {
        if (lobbyi != null && !string.IsNullOrEmpty(lobbyi.Id))
        {
            try
            {
                var lobby = await GetLobbyDetails(lobbyi.Id);
                // Check if the current player is the host
                bool isHost = AuthenticationService.Instance.PlayerId == lobby.HostId;
                // Check if there are at least two players in the lobby
                bool enoughPlayers = lobby.Players.Count >= 2;

                // Enable the Start button if the current player is the host and there are enough players
                startGameButton.gameObject.SetActive(isHost && enoughPlayers);
                startGameButton.interactable = isHost && enoughPlayers;
                if (lobby != null)
                {
                    ClearLobby();
                    foreach (Player player in lobby.Players)
                    {
                        GameObject item = Instantiate(lobbyItemPrefab, myLobbyList);
                        var itemScript = item.GetComponent<LobbyItem>();
                        if (itemScript != null)
                        {
                            itemScript.SetupMyLobby(player.Data["PlayerName"].Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error updating lobby UI: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Lobby is null or ID is not set.");
        }

    }

    private async void HandleLobbyUpdates(Lobby lobby)
    {
        lobbyUpdateTimer -= Time.deltaTime;
        if (lobbyUpdateTimer < 0f)
        {
            lobbyUpdateTimer = 30f; // Update every 30 seconds, adjust as needed
            await UpdateLobbyUI(lobby);
        }
    }


    private void ClearLobby()
    {
        foreach (Transform child in myLobbyList)
        {
            if (child == lobbyItemPrefab) continue;
            Destroy(child.gameObject);
        }
    }

    public async Task ListLobbies()
    {
        QueryLobbiesOptions options = new QueryLobbiesOptions
        {
            Count = 25,
            Filters = new List<QueryFilter>
            {
                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            },
            Order = new List<QueryOrder>
            {
                new QueryOrder(false, QueryOrder.FieldOptions.Created)
            }
        };

        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);
            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Transform child in lobbyListParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in queryResponse.Results)
            {
                GameObject item = Instantiate(lobbyItemPrefab, lobbyListParent);
                var itemScript = item.GetComponent<LobbyItem>();
                if (itemScript != null)
                {
                    itemScript.Setup(lobby.Name, lobby.Id);
                }
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("Error listing lobbies: " + e);
        }

        lobbyPanel.SetActive(false);
        lobbiesPanel.SetActive(true);
    }

    public void exitLobbies()
    {
        lobbyPanel.SetActive(true);
        lobbiesPanel.SetActive(false);
    }
    //-------------------------------------------
    private async void HandleLobbyHeartbeat()
    {
        if (hostLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;
                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }

    //---------------------------------------------
    public async Task<Lobby> GetLobbyDetails(string lobbyId)
    {
        try
        {
            Lobby lobby = await Lobbies.Instance.GetLobbyAsync(lobbyId);
            return lobby;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to fetch lobby details: " + ex.Message);
            return null;
        }
    }
    private void printPlayers(Lobby lobby)
    {
        Debug.Log("players in lobby :" + lobby.Name);
        foreach (Player player in lobby.Players)
        {
            Debug.Log(player.Id + " " + player.Data["PlayerName"].Value);

        }

    }

    private Coroutine requestThrottler;

    private IEnumerator ThrottleRequests(Action requestAction, float delay)
    {
        while (true)
        {
            requestAction.Invoke();
            yield return new WaitForSeconds(delay);
        }
    }


    private void StartHeartbeatTimer()
    {
        if (requestThrottler != null)
        {
            StopCoroutine(requestThrottler);
        }
        requestThrottler = StartCoroutine(ThrottleRequests(() => {
            if (hostLobby != null)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id).WrapErrors();
            }
        }, 30)); // Adjust the 30 seconds based on your rate limits
    }

    private void StopHeartbeatTimer()
    {
        if (requestThrottler != null)
        {
            StopCoroutine(requestThrottler);
            requestThrottler = null;
        }
    }

    private void OnDestroy()
    {
        if (requestThrottler != null)
        {
            StopCoroutine(requestThrottler);
        }
    }



    //----------------------------------------------------Relay----------------------------------------

    private async Task CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3); // Max number of players
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join code: " + joinCode);
            relayJoinCode.Value = joinCode;

            // Updating the metadata of the lobby
            Dictionary<string, DataObject> metadataUpdate = new Dictionary<string, DataObject>
        {
            {"RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode)}
        };

            // Assuming you have the lobbyId stored in hostLobby.Id
            await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                Data = metadataUpdate
            });

            // Setup network manager with relay data
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );
            
            NetworkManager.Singleton.StartHost();
            SpawnAllTargets();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }



    public void SpawnAllTargets()
    {
        foreach (var position in targetPositions)
        {
            // Instantiate the target prefab at each saved position
            GameObject targetObject = Instantiate(targetPrefab, position, Quaternion.identity);
            NetworkObject networkObject = targetObject.GetComponent<NetworkObject>();
            if (networkObject != null && NetworkManager.Singleton.IsServer)
            {
                networkObject.Spawn();
            }
            else
            {
                Debug.LogError("NetworkObject component not found or not on the server!");
            }
        }
    }
    //---------------------------------------------------------------------------


    private async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joing Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
                );
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

        }
    }

    private void StartGameAsHost()
    {
       
        var message = new FastBufferWriter(16, Unity.Collections.Allocator.Temp);
        message.WriteValueSafe(relayJoinCode.Value);
        NetworkManager.Singleton.SceneManager.LoadScene("MultiGame", UnityEngine.SceneManagement.LoadSceneMode.Single);

    }



    private void OnClientConnected(ulong clientId)
    {
        // This will log the client ID after it has connected to the server
        Debug.Log("Client connected with ID: " + clientId);

        // If this is our local client that has connected, log the LocalClientId
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log("This is our local client, ID: " + NetworkManager.Singleton.LocalClientId);
        }

        // Unsubscribe from the event to prevent it from being called multiple times
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

}

public static class TaskExtensions
{
    public static async void WrapErrors(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex}");
            // Handle rate limit errors specifically if needed
        }
    }


}