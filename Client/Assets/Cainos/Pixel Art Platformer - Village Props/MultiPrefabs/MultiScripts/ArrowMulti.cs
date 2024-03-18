using Unity.Netcode;
using UnityEngine;

public class ArrowMulti : NetworkBehaviour
{
    Rigidbody2D rb;
    bool hasHit;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!hasHit)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<NetworkObject>() != null)
        {
            ulong networkObjectId = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;

            if (IsServer)
            {
                ProcessCollision(networkObjectId);
            }
            else if (IsClient)
            {
                NotifyServerOfCollisionServerRpc(networkObjectId);
            }
        }

        hasHit = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }

    [ServerRpc]
    private void NotifyServerOfCollisionServerRpc(ulong networkObjectId)
    {
        ProcessCollision(networkObjectId);
    }

    private void ProcessCollision(ulong networkObjectId)
    {
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        Debug.Log("I got here to arrow multi ***");
        if (networkObject != null)
        {
            // If the hit object is a target, handle its destruction here.
            // For example:
             if (networkObject.CompareTag("Target"))
             {
                 networkObject.Despawn();
             }
        }
    }
}
