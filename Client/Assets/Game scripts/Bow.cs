using CodeMonkey.Utils;
using System.Collections;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float launchForce;
    public Transform shotPoint;
    Vector2 direction;

    public GameObject point;
    GameObject[] points;
    public int numberOfPoints;
    public float spaceBetweenPoints;
    private TextMeshProUGUI dataText;
    public const float MAX_FORCE = 20f;
    private SpriteMask forceSpriteMask;
    [SerializeField] private Transform forceTransform;
    private float holdDownStartTime;


    private void Start()
    {
        // Find the GameObject with the name "Data" in the scene
        GameObject targetsTextObject = GameObject.Find("Data");

        if (targetsTextObject != null)
        {
            // Try to get the Text component from the found GameObject
            dataText = targetsTextObject.GetComponent<TextMeshProUGUI>(); ;

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

    private void Awake()
    {
        forceSpriteMask = forceTransform.Find("mask").GetComponent<SpriteMask>();
        HideForce();
    }
    private void HideForce()
    {
        forceSpriteMask.alphaCutoff = 1;
    }
    public void ShowForce(float force)
    {
        forceSpriteMask.alphaCutoff = 1 - force / MAX_FORCE;
    }
    private float CalculateHoldDownForce(float holdTime)
    {
        float maxForceHoldDownTime = 2f;
        float holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        float force = holdTimeNormalized * MAX_FORCE;
        return force;
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.gameIsPaused == false)
        {
        Vector2 bowPosition = transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = mousePosition - bowPosition;
        transform.right = direction;
            if (Input.GetMouseButtonDown(0))
            {
                // Mouse Down, start holding
                holdDownStartTime = Time.time;
            }

            if (Input.GetMouseButton(0))
            {
                // Mouse still down, show force
                float holdDownTime = Time.time - holdDownStartTime;
                ShowForce(CalculateHoldDownForce(holdDownTime));
            }

            if (Input.GetMouseButtonUp(0))
            {
                // Mouse Up, Launch!
                float holdDownTime = Time.time - holdDownStartTime;
                Shoot(CalculateHoldDownForce(holdDownTime));
            }
        }
    }

    private void Shoot(float force)
    {
        // Convert gravity to local space
        Vector2 localGravity = transform.InverseTransformDirection(Physics2D.gravity);
        
        // Calculate the shooting angle
        float shootingAngle = Vector2.Angle(Vector2.right, transform.right);

        // Instantiate the arrow
        GameObject newArrow = Instantiate(arrowPrefab, shotPoint.position, shotPoint.rotation);
        Vector2 arrowVelocity = transform.right * force;  // Extract velocity without z-value
        newArrow.GetComponent<Rigidbody2D>().velocity = arrowVelocity;

        // Round the values for cleaner output
        shootingAngle = Mathf.Round(shootingAngle * 100f) / 100f;
        arrowVelocity = new Vector2(Mathf.Round(arrowVelocity.x * 100f) / 100f, Mathf.Round(arrowVelocity.y * 100f) / 100f);
        localGravity = new Vector2(Mathf.Round(localGravity.x * 100f) / 100f, Mathf.Round(localGravity.y * 100f) / 100f);

        // Calculate the equation with rounded values
        string equation = $"Data:\nAngle: {shootingAngle}\nVelocity: {arrowVelocity}\nGravity: {Physics2D.gravity}";

        dataText.text = equation;
        // Create a ShotPopup with the calculated equation
        //ShotPopup.Create(UtilsClass.GetMouseWorldPosition(), equation);

        // Destroy the arrow after a delay
        StartCoroutine(DestroyArrowAfterDelay(newArrow, 2f));
    }



    //NEWTON LAW FOR OBJECT SHOOTING
       // Vector2 position = (Vector2)shotPoint.position + (direction.normalized * launchForce * t) + 0.5f * Physics2D.gravity * (t * t);

    IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(arrow);
    }
}
