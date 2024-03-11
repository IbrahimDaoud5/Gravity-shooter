using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraControllerMultis : NetworkBehaviour
{
    [SerializeField] private NetworkObject player;

    void Update()
    {
        if (IsOwner)
        {
            // Check if the player is assigned and it's not null
            if (player != null)
            {
                // Ensure that the owner has authority over the player object
                if (player.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
                    Debug.Log("Camera following player: " + transform.position);
                }
            }
        }
    }
}
