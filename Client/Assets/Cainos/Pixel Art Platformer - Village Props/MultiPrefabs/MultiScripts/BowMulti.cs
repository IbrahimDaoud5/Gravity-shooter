using CodeMonkey.Utils;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class BowMulti : NetworkBehaviour
{
    public GameObject arrowPrefab;
    public float launchForce;
    public Transform shotPoint;
    Vector2 direction;
    public float rotationSpeed = 5f;

    private TextMeshProUGUI dataText;
    [SerializeField] private Camera myCamera;

    // Instead of Start
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

    void UpdateRotation()
    {
        Vector2 bowPosition = transform.position;
        //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //------
        /* Vector2 bowPosition = transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - bowPosition;
        transform.right = direction;*/
        if (myCamera != null)
        {
            Vector2 mousePosition = myCamera.ScreenToWorldPoint(Input.mousePosition);
            direction = mousePosition - bowPosition;
            transform.right = direction;
            Debug.Log("mouse position : *** " + direction);

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

    [ServerRpc]
    void UpdateServerRotationServerRpc(Quaternion targetRotation)
    {
        // Server sends the updated rotation to all clients
        UpdateClientRotationClientRpc(targetRotation);
    }

    [ClientRpc]
    void UpdateClientRotationClientRpc(Quaternion targetRotation)
    {
        // Clients receive the updated rotation from the server
        transform.rotation = targetRotation;
    }

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

    [ServerRpc]
    private void ServerRequestSpawnArrowServerRpc()
    {
        // Server receives a request from the client to spawn the arrow
        SpawnArrow();
    }

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

    IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(arrow);
    }
}
