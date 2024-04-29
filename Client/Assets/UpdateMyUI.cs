using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;

public class UpdateMyUI : NetworkBehaviour
{
    private TextMeshProUGUI targetsText;
    private TextMeshProUGUI DataText;
    private NetworkVariable<int> targetsHit = new NetworkVariable<int>();
    private NetworkVariable<FixedString128Bytes> data = new NetworkVariable<FixedString128Bytes>();
    private GameObject parentObject;

    public GameObject popupPanel;

    private void Start()
    {


        // Subscribe to the targetsHit value changed event to update the UI
        targetsHit.OnValueChanged += HandleTargetsValueChanged;
        data.OnValueChanged += HandleDataValueChanged;
    }
    public override void OnNetworkSpawn()
    {
        // Assign the TextMeshProUGUI component from the Canvas
        targetsText = GameObject.FindGameObjectWithTag("Targettxt").GetComponent<TextMeshProUGUI>();
        if (targetsText == null)
        {
            Debug.LogError("The Targettxt tag is not assigned to any TextMeshProUGUI components in this object's children.");
            return;
        }
        parentObject = GameObject.FindGameObjectWithTag("MultiCanvas");
        popupPanel = parentObject.transform.Find("PopupPanel").gameObject;
        if (popupPanel == null)
        {
            Debug.LogError("The popupPanel tag is not assigned to any object's children.");
            return;
        }
        DataText = GameObject.FindGameObjectWithTag("Datatxt").GetComponent<TextMeshProUGUI>();
        if (DataText == null)
        {
            Debug.LogError("The Targettxt tag is not assigned to any TextMeshProUGUI components in this object's children.");
            return;
        }
    }

    private void HandleDataValueChanged(FixedString128Bytes oldValue, FixedString128Bytes newValue)
    {
        // Only the owner should update their own UI
        if (IsOwner)
        {
            DataText.text = newValue.ToString();
        }
    }
    private void HandleTargetsValueChanged(int oldValue, int newValue)
    {
        // Only the owner should update their own UI
        if (IsOwner)
        {
            targetsText.text = "Targets: " + newValue;
        }
        if (newValue == 1 && IsOwner)
        {
            DisplayPopup();
        }
    }

    private void DisplayPopup()
    {
        if (popupPanel != null)
        {
            
            popupPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Popup panel is not assigned!");
        }
    }


    // Call this method when the player hits a target
    public void IncrementTargetHit()
    {
        if (IsServer)
        {
            // Increment on the server side and the value will be replicated to all clients
            targetsHit.Value += 1;
        }
        else
        {
            // Request the server to increment the counter
            IncrementTargetHitServerRpc();
        }
    }
    public void UpdateShotDataText(string data)
    {
        if (IsServer)
        {
            this.data.Value = new FixedString128Bytes(data);
        }
        else
        {
            UpdateShotDataServerRpc(data);
        }
    }

    
    [ServerRpc]
    public void UpdateShotDataServerRpc(string shotData)
    {
        data.Value = new FixedString128Bytes(shotData);
    }

    [ServerRpc]
    private void IncrementTargetHitServerRpc(ServerRpcParams rpcParams = default)
    {
        targetsHit.Value += 1;
    }
}
