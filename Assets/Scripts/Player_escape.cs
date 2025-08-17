using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Player_escape : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    CapsuleCollider2D coll;
    Vector2 coll_size;
    RaycastHit2D hit;
    Animator anim;
    bool isDucking = false;
    [SerializeField] private bool grounded = false;
    [SerializeField] private LayerMask groundLayer;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        coll_size = coll.size;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(speed, rb.velocity.y);
        rb.gravityScale = 1f;
        hit = Physics2D.Raycast(new Vector2(coll.bounds.center.x, coll.bounds.min.y)
            , Vector2.down, 0.1f, groundLayer);
        grounded = (hit.collider != null);

        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                //jump
                CancelInvoke("stopDuck");
                stopDuck();
                rb.velocity = new Vector2(speed, jumpForce);
                coll.size = new Vector2(coll_size.x, coll_size.y);
                coll.offset = Vector2.zero;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                if (!isDucking)
                {
                    //duck
                    coll.size = new Vector2(coll_size.x, coll_size.y / 2);
                    coll.offset = new Vector2(0, -0.5f);
                    isDucking = true;
                    Invoke("stopDuck", 1f);
                }
            }
        }
    }
    void stopDuck()
    {
        if (isDucking)
        {
            isDucking = false;
            coll.size = new Vector2(coll_size.x, coll_size.y);
            coll.offset = Vector2.zero;
        }
    }
}
