using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;


public class TargetHitMulti : NetworkBehaviour
{

    private Collider2D hitCollider;
    private bool hasBeenHit = false;

    [ServerRpc(RequireOwnership = false)]
    void HitTargetServerRpc(ulong shooterClientId)
    {
        if (!hasBeenHit)
        {
            hasBeenHit = true;

            // Find the player who hit the target
            var player = FindPlayerObject(shooterClientId);
            if (player != null)
            {
                // Call IncrementTargetHit on that player's UpdateMyUI script
                player.GetComponent<UpdateMyUI>().IncrementTargetHit();
            }
        }
        StartCoroutine(DestroyAfterDelay(hitCollider.gameObject));
    }

    private GameObject FindPlayerObject(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            return networkClient.PlayerObject.gameObject;
        }
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hitCollider = collision;
        if (IsClient && !hasBeenHit && collision.gameObject.CompareTag("Arrow"))
        {
            ArrowInfo arrowInfo = collision.GetComponent<ArrowInfo>(); 
            if (arrowInfo != null)
            {
                HitTargetServerRpc(arrowInfo.OwnerClientId);
            }
        }
    }

    IEnumerator DestroyAfterDelay(GameObject target)
    {
        yield return null; 


        if (IsServer || IsClient)
        {

            if (target.GetComponent<NetworkObject>() != null)
            {
                target.GetComponent<NetworkObject>().Despawn();
            }

            if (GetComponent<NetworkObject>() != null)
            {
                GetComponent<NetworkObject>().Despawn();
            }
        }
    }
}
