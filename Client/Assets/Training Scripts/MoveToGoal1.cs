using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MoveToGoal1 : Agent
{
    public GameObject bow;
    public Transform shootingPoint;
    public GameObject arrowPrefab;
    private bool canShoot = true; // Flag to control shooting
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector2 minBounds;
    [SerializeField] private Vector2 maxBounds;
    private int angle, force;

    public override void OnEpisodeBegin()
    {
        bow.transform.right = Vector2.zero;

        // Generate random position in half increments
        //Vector2 randomPosition = GenerateRandomPositionInHalves(minBounds, maxBounds);
        //targetTransform.transform.localPosition = new Vector2(randomPosition.x, randomPosition.y);

        canShoot = true;
    }

    public void GenerateRandomPositionInHalves()
    {
        int scaledXRange = (int)((maxBounds.x - minBounds.x) * 2);
        int scaledYRange = (int)((maxBounds.y - minBounds.y) * 2);

        int randomXScaled = Random.Range(0, scaledXRange + 1);
        int randomYScaled = Random.Range(0, scaledYRange + 1);

        float randomX = minBounds.x + (randomXScaled / 2.0f);
        float randomY = minBounds.y + (randomYScaled / 2.0f);

        // return new Vector2(randomX, randomY);

        //Vector2 randomPosition = GenerateRandomPositionInHalves(minBounds, maxBounds);
        targetTransform.transform.localPosition = new Vector2(randomX, randomY);
    }


    public override void CollectObservations(VectorSensor sensor)
    {

        // Agent's position relative to the target
        Vector2 toTarget = targetTransform.localPosition - shootingPoint.localPosition;
        sensor.AddObservation(toTarget.normalized); // Direction to target
        sensor.AddObservation(toTarget.magnitude);  // Distance to target
        sensor.AddObservation(targetTransform.localPosition.x);
        sensor.AddObservation(targetTransform.localPosition.y);
        sensor.AddObservation(shootingPoint.localPosition.x);
        sensor.AddObservation(shootingPoint.localPosition.y);

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!canShoot) return; // Do not shoot if canShoot is false

        // Decode actions into angle and force
        angle = actions.DiscreteActions[0];
        force = actions.DiscreteActions[1] + 10;

        // Debug.Log("force = " + force + " - angle = " + angle);


        canShoot = false;
        EndEpisode();

    }




    public int GetAngle()
    {
        return angle;
    }

    public int GetForce()
    {
        return force;
    }


}
