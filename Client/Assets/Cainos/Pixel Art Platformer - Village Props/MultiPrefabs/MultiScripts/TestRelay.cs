using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
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
    private GameObject playerUICanvasInstance;

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
            playerUICanvasInstance = Instantiate(playerUICanvasPrefab);


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
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);//here you can put the max number of players 
            /*this is the code you send to others so they can join the game*/
            string correctCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join code: " + correctCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
                );

            NetworkManager.Singleton.StartHost();
            SpawnAllTargets();
            //playerUICanvasInstance = Instantiate(playerUICanvasPrefab);

        }
        catch(RelayServiceException e) {
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
            NetworkManager.Singleton.StartClient();
        }
        catch(RelayServiceException e) {
            Debug.Log(e);

        }
    }
}
