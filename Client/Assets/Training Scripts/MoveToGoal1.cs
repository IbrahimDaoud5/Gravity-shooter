using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveToGoal1 : Agent
{

    public Transform shootingPoint;
    public int minStepsBetweenShots = 50;
    public Bow1 bowScript;


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
        var angle = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f) * 180 * Time.deltaTime; // Example range -90 to 90 degrees
        var force = Mathf.Clamp(actions.ContinuousActions[1], 0, 1) * 10; // Scale force within a range

        // Call your shooting mechanism here, using the decoded angle and force
        bowScript.Shoot(force, angle);

        // Add a small negative reward every step to encourage efficiency
        AddReward(-0.001f);
    }



}
