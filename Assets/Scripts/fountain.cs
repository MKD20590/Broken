using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fountain : MonoBehaviour
{
    Animator anim;
    [SerializeField] private Heart heart;
    bool isColliding = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isColliding && Input.GetKeyDown(KeyCode.E) && !heart.heartCompleted)
        {
            heart.completeHeart();
        }
        anim.SetBool("in", isColliding);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isColliding = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            isColliding = false;
        }
    }
}
