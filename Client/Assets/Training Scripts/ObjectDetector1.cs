using UnityEngine;

public class ObjectDetector1 : MonoBehaviour
{
    private Collider2D areaEffectorCollider;
    private bool bowObjectInside = false; // Track bow object's presence
    public RectTransform windArrowRectTransform;
    public float movementSpeed = 5f;
    public AreaEffector2D areaEffector;
    GameObject windArrow;

    private void Start()
    {

        windArrow.SetActive(false);
        areaEffectorCollider = GetComponent<Collider2D>(); // Cache AreaEffector2D collider



        if (areaEffectorCollider == null)
        {
            Debug.LogError("ObjectDetector requires an AreaEffector2D component attached to the same GameObject.");
            enabled = false; // Disable script if no collider found
            return;
        }


        areaEffectorCollider.isTrigger = true; // Ensure collider is a trigger
    }

    void Awake()
    {

        windArrow = GameObject.Find("WindArrow");
        // Debug.Log(windArrow);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bow"))
        {
            bowObjectInside = true;
            windArrow.SetActive(true);
            // Debug.Log("Bow entered the AreaEffector2D!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Bow"))
        {
            if (windArrow != null)
            {
                bowObjectInside = false;
                windArrow.SetActive(false);
                //Debug.Log("Bow exited the AreaEffector2D!");
            }
            else
            {
                Debug.LogWarning("windArrow reference is null in OnTriggerExit2D!");
            }
        }
    }



    private void Update()
    {
        if (bowObjectInside)
        {
            MoveWindArrow();

            // Debug.Log("Bow is currently inside the AreaEffector2D!");
        }
    }
    void MoveWindArrow()
    {
        if (areaEffector != null && windArrowRectTransform != null)
        {
            // Get the direction from the force applied by the AreaEffector2D
            float effectorForce = areaEffector.forceAngle;
            // Debug.Log(effectorForce);

            // Calculate the angle based on the direction vector
            float rotationAngle = effectorForce;


            // Rotate the wind arrow based on the calculated angle
            windArrowRectTransform.rotation = Quaternion.Euler(0, 0, rotationAngle - 90);
        }
    }



}
