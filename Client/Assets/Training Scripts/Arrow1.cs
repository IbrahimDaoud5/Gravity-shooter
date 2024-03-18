using UnityEngine;

public class Arrow1 : MonoBehaviour
{
    Rigidbody2D rb;
    bool hasHit;
    //public MoveToGoal1 agent;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (hasHit == false)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            //Debug.Log("Arrow Rotation Angle: " + angle);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("HIT");
            //agent.SetReward(15000f); // Reward for hitting the target
            FindObjectOfType<MoveToGoal1>().SetReward(+1f);
            FindObjectOfType<MoveToGoal1>().EndEpisode();


        }
        Destroy(gameObject);


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        hasHit = true;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }


        //Debug.Log("*");
        FindObjectOfType<MoveToGoal1>().SetReward(-0.1f);
        FindObjectOfType<MoveToGoal1>().EndEpisode();
        Destroy(gameObject);


    }
}
