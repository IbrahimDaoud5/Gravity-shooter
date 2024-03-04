using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraControllerMultis : NetworkBehaviour
{
    [SerializeField] private Transform player;
    void Update()
    {
        transform.position = new Vector3(player.position.x,player.position.y,transform.position.z);
    }
}
