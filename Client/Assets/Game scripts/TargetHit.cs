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
    public PauseMenu pauseMenu;

    private void Start()
    {
        // Find the GameObject with the name "TargetsText" in the scene
        GameObject targetsTextObject = GameObject.Find("TargetsText");
        GameObject canvas = GameObject.Find("Canvas");

        if (targetsTextObject != null)
        {
            // Try to get the Text component from the found GameObject
            targetsText = targetsTextObject.GetComponent<TextMeshProUGUI>();
            pauseMenu = canvas.GetComponent<PauseMenu>();

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

    private void Update()
    {
        if (targets >= 15)
        {
            pauseMenu.LoadMenu();
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
                targetsText.text = "Targets: " + targets + "/15";
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
