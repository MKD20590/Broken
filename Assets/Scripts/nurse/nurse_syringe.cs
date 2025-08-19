using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nurse_syringe : MonoBehaviour
{
    [SerializeField] private AudioSource dropSound;
    [SerializeField] private GameObject droplet;
    Animator anim;
    [SerializeField] private Transform posAwal;
    GameObject drop;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void attack()
    {
        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");
    }
    public void summonDroplet()
    {
        dropSound.Play();
        drop = Instantiate(droplet, posAwal.position ,transform.rotation);
    }
    public void clicked()
    {
        if(drop != null)
        {
            drop.GetComponent<nurse_droplet>().clicked();
        }
    }
}
