using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArrowMulti : NetworkBehaviour
{
    Rigidbody2D rb;
    bool hasHit;


   
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (hasHit==false)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            //Debug.Log("Arrow Rotation Angle: " + angle);
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
