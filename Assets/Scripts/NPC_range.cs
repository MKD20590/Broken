using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_range : MonoBehaviour
{
    [SerializeField] private float minDistance = 5f;
    GameObject player;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(player.transform.position, transform.position) > minDistance)
        {
            //detected
            //rb.velocity = player.transform.position - transform.position;
        }
        else
        {
            //rb.velocity *= 0;
        }
    }
}
