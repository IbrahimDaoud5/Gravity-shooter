using Unity.Netcode;
using UnityEngine;

public class ArrowMulti : NetworkBehaviour
{
    Rigidbody2D rb;
    bool hasHit;


    private void Start()
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

    private void OnCollisionEnter2D(Collision2D collision)
    {

        hasHit = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }

}
