using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Player_escape : MonoBehaviour
{
    [SerializeField] private GameObject shadow;
    escape_manager gm;
    Rigidbody2D rb;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    CapsuleCollider2D coll;
    Vector2 coll_offset;
    Vector2 coll_size;
    RaycastHit2D hit;
    Animator anim;
    bool isDucking = false;
    [SerializeField] private bool grounded = false;
    [SerializeField] private LayerMask groundLayer;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<escape_manager>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;
        coll = GetComponent<CapsuleCollider2D>();
        coll_size = coll.size;
        coll_offset = coll.offset;
    }

    // Update is called once per frame
    void Update()
    {
        shadow.transform.position = new Vector3(transform.position.x, shadow.transform.position.y, transform.position.z);
        rb.velocity = new Vector2(0, rb.velocity.y);
        hit = Physics2D.Raycast(new Vector2(coll.bounds.center.x, coll.bounds.min.y)
            , Vector2.down, 0.1f, groundLayer);
        grounded = (hit.collider != null);

        anim.SetBool("grounded", grounded);
        anim.SetBool("ducking", isDucking);
        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                //jump
                CancelInvoke("stopDuck");
                stopDuck();
                rb.velocity = new Vector2(speed, jumpForce);
                coll.size = new Vector2(coll_size.x, coll_size.y);
                coll.offset = coll_offset;
            }
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            if (!isDucking)
            {
                if(!grounded)
                {
                    //fall
                    rb.velocity = new Vector2(0, -30f);
                }
                //duck
                coll.size = new Vector2(coll_size.x, coll_size.y / 2);
                coll.offset = new Vector2(0, -0.45f);
                isDucking = true;
                //Invoke("stopDuck", 1.5f);
            }
        }
        else if(Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            if (isDucking)
            {
                stopDuck();
                //Invoke("stopDuck", 0.5f);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("obstacle"))
        {
            gm.getHit();
            collision.GetComponent<PolygonCollider2D>().enabled = false;
            collision.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collision.GetComponent<SpriteRenderer>().enabled = false;
            if(collision.transform.childCount > 1)
            {
                collision.transform.GetChild(1).gameObject.SetActive(false);
            }
        }
    }
    void stopDuck()
    {
        if (isDucking)
        {
            isDucking = false;
            coll.size = new Vector2(coll_size.x, coll_size.y);
            coll.offset = coll_offset;
        }
    }
}
