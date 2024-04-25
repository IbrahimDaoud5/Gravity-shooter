using System.Collections;
using TMPro;
using UnityEngine;

public class Bow1 : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float launchForce;
    public Transform shotPoint;
    Vector2 direction;

    public GameObject point;
    public int numberOfPoints;
    public float spaceBetweenPoints;
    private TextMeshProUGUI dataText;
    public const float MAX_FORCE = 20f;
    private SpriteMask forceSpriteMask;
    [SerializeField] private Transform forceTransform;
    private float holdDownStartTime;
    //private float angle, force;




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
        float maxForceHoldDownTime = 0.7f;
        float holdTimeNormalized = Mathf.Clamp01(holdTime / maxForceHoldDownTime);
        float force = holdTimeNormalized * MAX_FORCE;
        return force;

    }

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

    public void Shoot(float force)
    {
        // Convert gravity to local space
        Vector2 localGravity = transform.InverseTransformDirection(Physics2D.gravity);

        Vector2 toRight = Vector2.right;
        float shootingAngle = Vector2.Angle(toRight, transform.right);


        // Determine if the angle should be adjusted to account for direction
        if (Vector3.Cross(toRight, transform.right).z < 0)
        {
            shootingAngle = 360 - shootingAngle;
        }

        //Debug.Log("actual angle = " + shootingAngle + " - actual force = " + force);

        // Instantiate the arrow
        GameObject newArrow = Instantiate(arrowPrefab, shotPoint.position, shotPoint.rotation);
        Arrow1 arrowScript = newArrow.GetComponent<Arrow1>();

        Vector2 arrowVelocity = transform.right * force;  // Extract velocity without z-value
        newArrow.GetComponent<Rigidbody2D>().velocity = arrowVelocity;

        if (arrowScript != null)
        {
            arrowScript.SetInitialValues(shootingAngle, force);
            arrowScript.agent = FindObjectOfType<MoveToGoal1>();  // Assign the agent to the arrow
        }



        // Destroy the arrow after a delay
        // StartCoroutine(DestroyArrowAfterDelay(newArrow, 0.3f));
    }



    //NEWTON LAW FOR OBJECT SHOOTING
    // Vector2 position = (Vector2)shotPoint.position + (direction.normalized * launchForce * t) + 0.5f * Physics2D.gravity * (t * t);

    IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(arrow);
    }



    //public float GetAngle()
    //{
    //    return angle;
    //}

    //public float GetForce()
    //{
    //    return force;
    //}
}
