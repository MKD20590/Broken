using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC_Range : MonoBehaviour
{
    AudioSource whispers;
    [SerializeField] private float minDistance = 5f;
    GameObject player;
    Animator anim;
    Rigidbody2D rb;
    [SerializeField] private bool isManager = false;
    List<NPC_Range> npcs;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        if(isManager)
        {
            npcs = new List<NPC_Range>();
            foreach(NPC_Range n in FindObjectsOfType<NPC_Range>())
            {
                npcs.Add(n);
            }
        }
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        whispers = GameObject.Find("whispers").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        speed = minDistance - Vector2.Distance(player.transform.position, transform.position);
        if (speed > 1)
        {
            speed = 1;
        }
        else if (speed < 0)
        {
            speed = 0;
        }
        if (isManager)
        {
            List<float> temp = new List<float>();
            for (int i = 0; i < npcs.Count; i++)
            {
                temp.Add(npcs[i].speed);
            }
            ;
            whispers.volume = temp.Max();
        }
        anim.SetFloat("speed", speed);
    }
}
