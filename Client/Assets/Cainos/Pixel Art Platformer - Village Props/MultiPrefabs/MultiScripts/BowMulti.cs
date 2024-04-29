using CodeMonkey.Utils;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Unity.VisualScripting;

public class BowMulti : NetworkBehaviour
{
    // Reference to the arrow prefab
    public GameObject arrowPrefab;

    // Transform where the arrow is spawned
    public Transform shotPoint;
    // Direction from bow to mouse
    Vector2 direction;
    // Rotation speed of the bow
    public float rotationSpeed = 5f;

    // UI element to display data
    private TextMeshProUGUI dataText;
    // Reference to the player's camera
    [SerializeField] private Camera myCamera;
    private float MAX_FORCE = 20F;
    
    private SpriteMask forceSpriteMask;
    [SerializeField] private Transform forceTransform;
    private float holdDownStartTime;

    public GameObject popupPanel;
    private GameObject parentObject;

    // Called when the player object is spawned in the network
    public override void OnNetworkSpawn()
    {

        // Find the GameObject with the name "Data" in the scene
        GameObject targetsTextObject = GameObject.FindGameObjectWithTag("Datatxt");
        parentObject = GameObject.FindGameObjectWithTag("MultiCanvas");

        if (targetsTextObject != null)
        {
            // Try to get the Text component from the found GameObject
            dataText = targetsTextObject.GetComponent<TextMeshProUGUI>();

            if (dataText == null)
            {
                Debug.LogError("data does not have a Text component!");
            }
        }
        else
        {
            Debug.LogError("data GameObject not found in the scene!");
        }
    }

    public void OnButtonForceClicked()
    {
        ChangeFroce();
    }
    
    
    public void OnButtonGravityClicked()
    {
        ChangeGravityServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void ChangeFroce()
    {

            MAX_FORCE= 50f;
            popupPanel.SetActive(false);
    }



    // Server RPC that is called by the client
    [ServerRpc(RequireOwnership = false)]
    private void ChangeGravityServerRpc(ulong requesterClientId)
    {
        ChangeGravityClientRpc(requesterClientId, -20f);
    }

    // Client RPC that is executed on all clients
    [ClientRpc]
    private void ChangeGravityClientRpc(ulong requesterClientId, float newGravityY)
    {
        // Check if the executing client is the one who requested the change
        if (NetworkManager.Singleton.LocalClientId != requesterClientId)
        {
            Physics2D.gravity = new Vector2(Physics2D.gravity.x, newGravityY);
        }
        popupPanel.SetActive(false);
    }



    private void Awake()
    {
        forceSpriteMask = forceTransform.Find("mask").GetComponent<SpriteMask>();
        HideForce();

    }
    private void HideForce()
    {
        forceSpriteMask.alphaCutoff = 1;
    }
    public void ShowForce(float force)
    {
        forceSpriteMask.alphaCutoff = 1 - force / MAX_FORCE;
    }
    private float CalculateHoldDownForce(float holdTime)
    {
        float maxForceHoldDownTime = 0.7f;
        float holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        float force = holdTimeNormalized * MAX_FORCE;
        return force;
    }

    // Initialize camera distance
    private float distanceToCamera;
    private CinemachineBrain cameraBrain;

    private void Start()
    {
        popupPanel = parentObject.transform.Find("PopupPanel").gameObject;
        if (popupPanel != null)
        {
            // Find the button as a child of the panel by name and add a listener
            Button buttonForce = popupPanel.transform.Find("Force").GetComponent<Button>();
            if (buttonForce != null)
            {
                buttonForce.onClick.AddListener(OnButtonForceClicked);
            }
            else
            {
                Debug.LogError("Button not found in panel!");
            }
            Button buttonGravity = popupPanel.transform.Find("Gravity").GetComponent<Button>();
            if (buttonGravity != null)
            {
                buttonGravity.onClick.AddListener(OnButtonGravityClicked);
            }
            else
            {
                Debug.LogError("Button not found in panel!");
            }
        }
        else
        {
            Debug.LogError("Panel not assigned!");
        }
        // Get the CinemachineBrain from the main camera
        cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        // Add a listener for camera cut events
        cameraBrain.m_CameraCutEvent.AddListener((brain) =>
        {
            if (brain != null)
            {
                if (brain.ActiveVirtualCamera != null)
                {
                    // if virtual camera changed
                    distanceToCamera = Vector3.Distance(transform.position, brain.ActiveVirtualCamera.VirtualCameraGameObject.transform.position);
                    Debug.Log($"on cut event {brain.ActiveVirtualCamera.Name} {distanceToCamera}");
                }
            }
        });

        // Init distance
        distanceToCamera = Vector3.Distance(transform.position, Camera.main.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (PauseMenu.gameIsPaused == false)
        {
            UpdateRotation();

            if (Input.GetMouseButtonDown(0))
            {
                // Mouse Down, start holding
                holdDownStartTime = Time.time;
            }

            if (Input.GetMouseButton(0))
            {
                // Mouse still down, show force
                float holdDownTime = Time.time - holdDownStartTime;
                ShowForce(CalculateHoldDownForce(holdDownTime));
            }

            if (Input.GetMouseButtonUp(0))
            {
                // Mouse Up, Launch!
                float holdDownTime = Time.time - holdDownStartTime;
                Shoot(CalculateHoldDownForce(holdDownTime));
            }
        }
    }

    // Update the bow's rotation based on mouse position
    void UpdateRotation()
    {
        if (myCamera != null)
        {
            // Convert mouse position to world point
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceToCamera));
            // Calculate direction from bow to mouse
            direction = mousePosition - transform.position;
            // Set bow rotation to face the mouse direction
            transform.right = direction;

        }
        else
        {
            Debug.LogError("Player camera reference is null. Make sure to assign it in the inspector.");
        }

        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Smoothly interpolate the rotation for better visual results
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // Ensure the server sends the updated rotation to clients
            UpdateServerRotationServerRpc(targetRotation);
        }
    }

    // Server RPC to update rotation on server
    [ServerRpc]
    void UpdateServerRotationServerRpc(Quaternion targetRotation)
    {
        // Server sends the updated rotation to all clients
        UpdateClientRotationClientRpc(targetRotation);
    }

    // Client RPC to update rotation on client
    [ClientRpc]
    void UpdateClientRotationClientRpc(Quaternion targetRotation)
    {
        // Clients receive the updated rotation from the server
        transform.rotation = targetRotation;
    }

    // Function to handle shooting logic
    private void Shoot(float force)
    {
        if (IsServer)
        {
            // Spawn the arrow with server's authority (will change later to the client's)
            SpawnArrow(force, NetworkManager.ServerClientId);
        }
        else
        {
            // If this is a client, notify the server to spawn the arrow
            ServerRequestSpawnArrowServerRpc(force, NetworkManager.Singleton.LocalClientId);
        }
    }

    // Server RPC to handle client request for arrow spawn
    // Server RPC to handle client request for arrow spawn
    [ServerRpc]
    private void ServerRequestSpawnArrowServerRpc(float force, ulong clientId)
    {
        // Server receives a request from the client to spawn the arrow
        // Pass along the client's ID so the arrow is spawned with the correct ownership
        SpawnArrow(force, clientId);
    }


    // Function to spawn an arrow
    private void SpawnArrow(float force, ulong ownerId)
    {
        // Convert gravity to local space
        Vector2 localGravity = transform.InverseTransformDirection(Physics2D.gravity);

        // Calculate the shooting angle
        float shootingAngle = Vector2.Angle(Vector2.right, transform.right);

        // Instantiate the arrow
        GameObject newArrow = Instantiate(arrowPrefab, shotPoint.position, shotPoint.rotation);
        NetworkObject arrowNetworkObject = newArrow.GetComponent<NetworkObject>();
        Vector2 arrowVelocity = transform.right * force;
        newArrow.GetComponent<Rigidbody2D>().velocity = arrowVelocity;

        if (arrowNetworkObject != null)
        {
            // Set the owner client ID on the arrow
            ArrowInfo arrowInfo = newArrow.GetComponent<ArrowInfo>();
            if (arrowInfo != null)
            {
                arrowInfo.SetOwnerClientId(ownerId);
            }
            // Spawn the arrow on the network with the client's ownership
             
            arrowNetworkObject.SpawnWithOwnership(ownerId);
        }
        // Calculate the equation with rounded values
        string equation = $"Data:\nAngle: {Mathf.Round(shootingAngle * 100f) / 100f} degrees\n" +
                          $"Velocity: {Mathf.Round(arrowVelocity.magnitude * 100f) / 100f} m/s\n" +
                          $"Gravity: {Mathf.Round(Physics2D.gravity.y * 100f) / 100f} m/s²";

        // Assuming this script is on the player prefab, which has the NetworkObject
        var player = FindPlayerObject(ownerId);
        if (player != null)
        {
            // Call IncrementTargetHit on that player's UpdateMyUI script
            player.GetComponent<UpdateMyUI>().UpdateShotDataText(equation);
        }
        StartCoroutine(DestroyArrowAfterDelay(newArrow, 2f));
    }
    private GameObject FindPlayerObject(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            return networkClient.PlayerObject.gameObject;
        }
        return null;
    }

    // Coroutine to destroy arrow after a delay
    IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {

        yield return new WaitForSeconds(delay);
        Destroy(arrow);
    }
}
