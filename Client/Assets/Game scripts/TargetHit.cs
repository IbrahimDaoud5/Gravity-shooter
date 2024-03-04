using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetHit : MonoBehaviour
{
    private static int targets = 0;

    private TextMeshProUGUI targetsText;
    private bool hasBeenHit = false;

    private void Start()
    {
        // Find the GameObject with the name "TargetsText" in the scene
        GameObject targetsTextObject = GameObject.Find("TargetsText");

        if (targetsTextObject != null)
        {
            // Try to get the Text component from the found GameObject
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasBeenHit && collision.gameObject.CompareTag("Target"))
        {
            hasBeenHit = true;

            targets++;

            if (targetsText != null)
            {
                targetsText.text = "Targets: " + targets;
                /*adding the 5 shots change gravity feature
                Vector2 newGravity = new Vector2(0f, 0f);
                Physics2D.gravity = newGravity;
                */

            }

            StartCoroutine(DestroyAfterDelay(collision.gameObject));
        }
    }

    IEnumerator DestroyAfterDelay(GameObject target)
    {
        yield return null; // Wait for the next frame

        // Destroy the target and the arrow
        Destroy(target);
        Destroy(gameObject);
    }
    public static void SetTargets(int value)
    {
        targets = value;
    }
}
