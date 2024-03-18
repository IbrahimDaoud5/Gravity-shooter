using CodeMonkey.Utils;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class BowMulti : NetworkBehaviour
{
    // Reference to the arrow prefab
    public GameObject arrowPrefab;
    // Force applied when launching the arrow
    public float launchForce;
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

    // Called when the player object is spawned in the network
    public override void OnNetworkSpawn()
    {
        // Find the GameObject with the name "Data" in the scene
        GameObject targetsTextObject = GameObject.Find("Data");

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

    // Initialize camera distance
    private float distanceToCamera;
    private CinemachineBrain cameraBrain;

    private void Start()
    {
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
                Shoot();
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
    private void Shoot()
    {
        if (IsServer)
        {
            // Only the server should spawn the arrow
            SpawnArrow();
        }
        else
        {
            // If this is a client, notify the server to spawn the arrow
            ServerRequestSpawnArrowServerRpc();
        }
    }

    // Server RPC to handle client request for arrow spawn
    [ServerRpc]
    private void ServerRequestSpawnArrowServerRpc()
    {
        // Server receives a request from the client to spawn the arrow
        SpawnArrow();
    }

    // Function to spawn an arrow
    private void SpawnArrow()
    {
        // Convert gravity to local space
        Vector2 localGravity = transform.InverseTransformDirection(Physics2D.gravity);

        // Calculate the shooting angle
        float shootingAngle = Vector2.Angle(Vector2.right, transform.right);

        // Instantiate the arrow
        GameObject newArrow = Instantiate(arrowPrefab, shotPoint.position, shotPoint.rotation);
        Vector2 arrowVelocity = transform.right * launchForce;
        newArrow.GetComponent<Rigidbody2D>().velocity = arrowVelocity;

        // Spawn the arrow on the network
        newArrow.GetComponent<NetworkObject>().Spawn(true);

        // Round the values for cleaner output
        shootingAngle = Mathf.Round(shootingAngle * 100f) / 100f;
        arrowVelocity = new Vector2(Mathf.Round(arrowVelocity.x * 100f) / 100f, Mathf.Round(arrowVelocity.y * 100f) / 100f);
        localGravity = new Vector2(Mathf.Round(localGravity.x * 100f) / 100f, Mathf.Round(localGravity.y * 100f) / 100f);

        // Calculate the equation with rounded values
        string equation = $"Data:\nAngle: {shootingAngle}\nVelocity: {arrowVelocity}\nGravity: {Physics2D.gravity}";

        dataText.text = equation;

        // Destroy the arrow after a delay
        StartCoroutine(DestroyArrowAfterDelay(newArrow, 2f));
    }

    // Coroutine to destroy arrow after a delay
    IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {
       
        yield return new WaitForSeconds(delay);
        Destroy(arrow);
    }
}
