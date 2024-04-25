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

    //public override void OnEpisodeBegin()
    //{
    //    bow.transform.right = Vector2.zero;
    //    float newX = Random.Range(minBounds.x, maxBounds.x);
    //    float newY = Random.Range(minBounds.y, maxBounds.y);
    //    targetTransform.transform.localPosition = new Vector2(newX, newY);

    //    canShoot = true;
    //}
    public override void OnEpisodeBegin()
    {
        bow.transform.right = Vector2.zero;

        // Generate random position in half increments
        //Vector2 randomPosition = GenerateRandomPositionInHalves(minBounds, maxBounds);
        //targetTransform.transform.localPosition = new Vector2(randomPosition.x, randomPosition.y);


        // Generate a random position for the player 
        //Vector2 randomPlayerPosition = GenerateRandomPositionInHalves(minBounds, maxBounds);
        // this.transform.localPosition = new Vector2(randomPlayerPosition.x, randomPlayerPosition.y);

        canShoot = true;
    }

    private Vector2 GenerateRandomPositionInHalves(Vector2 min, Vector2 max)
    {
        int scaledXRange = (int)((max.x - min.x) * 2);
        int scaledYRange = (int)((max.y - min.y) * 2);

        int randomXScaled = Random.Range(0, scaledXRange + 1);
        int randomYScaled = Random.Range(0, scaledYRange + 1);

        float randomX = min.x + (randomXScaled / 2.0f);
        float randomY = min.y + (randomYScaled / 2.0f);

        return new Vector2(randomX, randomY);
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
        angle = actions.DiscreteActions[0];// Mathf.Clamp(actions.ContinuousActions[0], 0, 1f) * 90; // Angle range 0 to 360 degrees
        force = actions.DiscreteActions[1] + 10;//Mathf.Clamp(actions.ContinuousActions[1], 0.5f, 1f) * 20; // Scale force within a reasonable range

        // Debug.Log("force = " + force + " - angle = " + angle);

        ////Debug.Log(actions.ContinuousActions[1]);
        //// Calculate the angle in radians for direction calculation
        //float angleInRadians = angle * Mathf.Deg2Rad;

        //// Calculate the direction vector from the angle
        //Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

        //// Adjust bow's rotation to face the calculated direction
        //bow.transform.right = direction;

        //// Instantiate the arrow at the shooting point
        //GameObject newArrow = Instantiate(arrowPrefab, shootingPoint.position, Quaternion.Euler(0, 0, angle)); // Adjusting the angle to match the direction

        //// Apply the calculated force as velocity to the arrow
        //newArrow.GetComponent<Rigidbody2D>().velocity = direction * force;
        //newArrow.GetComponent<Arrow1>().agent = this;


        canShoot = false;
        //EndEpisode();

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
