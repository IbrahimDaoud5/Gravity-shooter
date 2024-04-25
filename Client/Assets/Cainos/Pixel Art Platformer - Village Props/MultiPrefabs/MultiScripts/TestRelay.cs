using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class TestRelay : NetworkBehaviour
{
    public GameObject targetPrefab;
    private List<Vector3> targetPositions;
    public SpawnPosition spawnPosition;
    [SerializeField] private Button clientBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private TMP_InputField codeInputField;
    public GameObject playerUICanvasPrefab;

    // Start is called before the first frame update
    private async void Start()
    {
        targetPositions = spawnPosition.savedPositions;
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        hostBtn.onClick.AddListener(() =>
        {
            CreateRelay();
        });
        clientBtn.onClick.AddListener(() =>
        {
            
                // If correct, send the code to JoinRelay function
                string enteredCode = codeInputField.text;
            enteredCode = enteredCode.Substring(0, 6);
            JoinRelay(enteredCode);
           /* GameObject playerUI = Instantiate(playerUICanvasPrefab);
            PlayerUIManager uiManager = playerUI.GetComponent<PlayerUIManager>();*/



        });
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

    private async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3); // Max number of players
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join code: " + joinCode);

            // Create lobby with relay join code in its data for easy retrieval by clients
            var lobbyData = new Dictionary<string, Unity.Services.Lobbies.Models.DataObject>
        {
            {"relayJoinCode", new Unity.Services.Lobbies.Models.DataObject(Unity.Services.Lobbies.Models.DataObject.VisibilityOptions.Public, joinCode)}
        };
            var options = new CreateLobbyOptions { Data = lobbyData };
            Unity.Services.Lobbies.Models.Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("MyLobby", 3, options);

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
        catch(RelayServiceException e) {
            Debug.Log(e);

        }
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
