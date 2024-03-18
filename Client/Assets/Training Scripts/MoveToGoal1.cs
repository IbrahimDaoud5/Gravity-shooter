using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveToGoal1 : Agent
{
    public GameObject bow;
    public Transform shootingPoint;
    public int minStepsBetweenShots = 50;
    public GameObject arrowPrefab;


    public override void OnEpisodeBegin()
    {
        // bow.transform.right = Vector2.zero;
    }

    [SerializeField] private Transform targetTransform;
    public override void CollectObservations(VectorSensor sensor)
    {

        // Agent's position relative to the target
        Vector3 toTarget = targetTransform.position - shootingPoint.position;
        sensor.AddObservation(toTarget.normalized); // Direction to target
        sensor.AddObservation(toTarget.magnitude);  // Distance to target
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Decode actions into angle and force
        var angle = Mathf.Clamp(actions.ContinuousActions[0], 0, 1f) * 90 + 90; // Angle range 0 to 360 degrees
        var force = Mathf.Clamp(actions.ContinuousActions[1], 0, 1f) * 20; // Scale force within a reasonable range

        //Debug.Log(actions.ContinuousActions[0]);
        //Debug.Log(actions.ContinuousActions[1]);
        // Calculate the angle in radians for direction calculation
        float angleInRadians = angle * Mathf.Deg2Rad;

        // Calculate the direction vector from the angle
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

        // Adjust bow's rotation to face the calculated direction
        bow.transform.right = direction;

        // Instantiate the arrow at the shooting point
        GameObject newArrow = Instantiate(arrowPrefab, shootingPoint.position, Quaternion.Euler(0, 0, angle - 90)); // Adjusting the angle to match the direction

        // Apply the calculated force as velocity to the arrow
        newArrow.GetComponent<Rigidbody2D>().velocity = direction * force;


        StartCoroutine(DestroyArrowAfterDelay(newArrow, 4f));
    }



    IEnumerator DestroyArrowAfterDelay(GameObject arrow, float delay)
    {
        //SetReward(-1f);
        // EndEpisode();
        yield return new WaitForSeconds(delay);
        Destroy(arrow);
    }




}
