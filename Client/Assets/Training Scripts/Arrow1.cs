using System;
using UnityEngine;
public class Arrow1 : MonoBehaviour
{
    Rigidbody2D rb;
    bool hasHit;
    public MoveToGoal1 agent;  // The ML agent
    private float actualAngle;
    private float actualForce;
    public PopupController popupController;

    void Start()
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

    public void SetInitialValues(float angle, float force)
    {
        actualAngle = angle;
        actualForce = force;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Target"))
        {
            Destroy(gameObject);
            Debug.Log("HIT");
            agent.GenerateRandomPositionInHalves();

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        if (!collision.gameObject.CompareTag("Target"))
        {
            ProvideFeedback();
        }
        Destroy(gameObject);
    }

    private void ProvideFeedback()
    {
        string msg = "";
        int modelAngle = agent.GetAngle();
        int modelForce = agent.GetForce();

        int maxAngle = 360; // Maximum possible angle
        int maxForce = 20;  // Maximum possible force


        double angleDifferencePercent = Math.Abs((double)(actualAngle - modelAngle) / maxAngle) * 100;
        double forceDifferencePercent = Math.Abs((double)(actualForce - modelForce) / maxForce) * 100;

        // Apply weights
        double weightedAngleDifference = angleDifferencePercent * 0.7;
        double weightedForceDifference = forceDifferencePercent * 0.3;

        Debug.Log("actual angle = " + actualAngle + " , Force = " + actualForce);
        Debug.Log("model angle = " + modelAngle + " , Force = " + modelForce);

        // Define thresholds for small, moderate, and large differences
        double smallThreshold = 20;
        double moderateThreshold = 35;


        if (weightedAngleDifference > weightedForceDifference)
        {
            if (actualAngle < modelAngle)
            {
                if (angleDifferencePercent > moderateThreshold)
                    msg = "Significantly increase ANGLE";
                else if (angleDifferencePercent > smallThreshold)
                    msg = "Moderately increase ANGLE";
                else
                    msg = "Slightly increase ANGLE";
            }
            else if (actualAngle > modelAngle)
            {
                if (angleDifferencePercent > moderateThreshold)
                    msg = "Significantly decrease ANGLE";
                else if (angleDifferencePercent > smallThreshold)
                    msg = "Moderately decrease ANGLE";
                else
                    msg = "Slightly decrease ANGLE";
            }
        }
        else if (weightedForceDifference > weightedAngleDifference)
        {
            if (actualForce < modelForce)
            {
                if (forceDifferencePercent > moderateThreshold)
                    msg = "Significantly increase FORCE";
                else if (forceDifferencePercent > smallThreshold)
                    msg = "Moderately increase FORCE";
                else
                    msg = "Slightly increase FORCE";
            }
            else if (actualForce > modelForce)
            {
                if (forceDifferencePercent > moderateThreshold)
                    msg = "Significantly decrease FORCE";
                else if (forceDifferencePercent > smallThreshold)
                    msg = "Moderately decrease FORCE";
                else
                    msg = "Slightly decrease FORCE";
            }
        }
        else
        {
            if (actualAngle != modelAngle)
            {
                msg = actualAngle < modelAngle ? "Adjust ANGLE: Increase" : "Adjust ANGLE: Decrease";
            }
            else if (actualForce != modelForce)
            {
                msg = actualForce < modelForce ? "Adjust FORCE: Increase" : "Adjust FORCE: Decrease";
            }
        }

        Debug.Log(msg);
        ShotPopup.Create(new Vector3(2, -1, 0), msg);
    }








}
