using System.Collections;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class TargetHitMulti : NetworkBehaviour
{
    private static int targets = 0;
    public GameObject targetPrefab;
    private Collider2D hitCollider;
    private TextMeshProUGUI targetsText;
    private bool hasBeenHit = false;

    private void Start()
    {
        if (IsClient) // Only clients need to find and set the UI component
        {
            GameObject targetsTextObject = GameObject.Find("TargetsText");
            if (targetsTextObject != null)
            {
                targetsText = targetsTextObject.GetComponent<TextMeshProUGUI>();
                if (targetsText == null)
                {
                    Debug.LogError("TargetsText does not have a Text component!");
                }
            }
            else
            {
                Debug.LogError("TargetsText GameObject not found in the scene!");
            }
        }

        
    }
    
 

    [ServerRpc(RequireOwnership = false)]
    void HitTargetServerRpc()
    {
        Debug.Log("targets check : " + targets + " hit collidr check :" + hitCollider);
        if (!hasBeenHit)
        {
            hasBeenHit = true;
            targets++;
            targetsText.text = "Targets: " + targets;
            Debug.Log("hit at server rpc :" + targets);
            StartCoroutine(DestroyAfterDelay(hitCollider.gameObject));
            // Additional server-side validation and logic here
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        hitCollider = collision;
        if (IsClient && !hasBeenHit && collision.gameObject.CompareTag("Target"))
        {
            HitTargetServerRpc(); // Tell the server a target has been hit
            //StartCoroutine(DestroyAfterDelay(collision.gameObject));
            
        }
    }

    IEnumerator DestroyAfterDelay(GameObject target)
    {
        yield return null; // Wait for the next frame


        // Needs authority to destroy objects, consider handling this on the server or with proper network object destruction
        if (IsServer||IsClient)
        {
           // Debug.Log("target to destroy : " + target.GetComponent<NetworkObject>());
           // Debug.Log("Object to destroy : : " + GetComponent<NetworkObject>());
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
