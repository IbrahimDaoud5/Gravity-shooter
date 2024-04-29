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

        Debug.Log("actual angle = " + actualAngle + " , Force = " + actualForce);
        Debug.Log("model angle = " + modelAngle + " , Force = " + modelForce);

        // Check angle feedback
        if (actualAngle < modelAngle)
        {
            msg = "Increase ANGLE";
        }
        else if (actualAngle > modelAngle)
        {
            msg = "Decrease ANGLE";
        }

        // Check force feedback
        if (actualForce < modelForce)
        {
            msg += msg.Length > 0 ? " & Increase FORCE" : "Increase FORCE";
        }
        else if (actualForce > modelForce)
        {
            msg += msg.Length > 0 ? " & Decrease FORCE" : "Decrease FORCE";
        }

        Debug.Log(msg);
        //popupController.ShowPopup(msg, 3.0f);
        ShotPopup.Create(new Vector3(2, -1, 0), msg);
    }







}
