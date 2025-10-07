using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class NurseDroplet : MonoBehaviour
{
    NurseGM gm;
    Nurse nurse;
    GameObject healthBar;
    GameObject clicker;
    ParticleSystem dies;
    string score = "";
    [SerializeField] private float speed;
    [SerializeField] private Vector3 arah;
    Rigidbody2D rb;
    bool fail = false;
    bool isWinning = false;
    TrailRenderer trail;
    bool canClick = true;
    // Start is called before the first frame update
    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        healthBar = GameObject.Find("HP_bar");
        clicker = GameObject.Find("CLICKER");
        rb = GetComponent<Rigidbody2D>();
        dies = transform.GetChild(0).GetComponent<ParticleSystem>();
        gm = FindObjectOfType<NurseGM>();
        nurse = FindObjectOfType<Nurse>();
        arah = (clicker.transform.position - transform.position).normalized;
        rb.velocity = Vector2.right * 15f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWinning)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.7f, Time.deltaTime * 1f);
            trail.widthMultiplier = Mathf.Lerp(trail.widthMultiplier, 0.3f,Time.deltaTime * 1f);
            if (!fail && Vector2.Distance(transform.position, clicker.transform.position) > 0.01f)
            {
                arah = (clicker.transform.position - transform.position).normalized;
            }
            else if(!fail && Vector2.Distance(transform.position, clicker.transform.position) <= 0.01f)
            {
                fail = true;
            }
            else if (fail)
            {
                arah = (healthBar.transform.position - transform.position).normalized;
            }
            rb.velocity = Vector2.MoveTowards(rb.velocity, arah * speed, Time.deltaTime * 20f);
        }
    }
    public void Clicked()
    {
        if(canClick)
        {
            if (Vector2.Distance(transform.position, clicker.transform.position) <= 0.35f)
            {
                isWinning = true;
                score = "perfect";
                Debug.Log("perfect");
                clicker.transform.GetChild(1).GetComponent<TextMeshPro>().text = "PERFECT!";
                clicker.transform.GetChild(1).GetComponent<TextMeshPro>().color = Color.white;
                clicker.GetComponent<Animator>().ResetTrigger("hit");
                clicker.GetComponent<Animator>().SetTrigger("hit");
                GetComponent<SpriteRenderer>().color = Color.white;
                arah = (nurse.transform.position - transform.position).normalized;
                rb.velocity = arah * 30;
                trail.startColor = Color.white;
                ParticleSystem.MainModule main = dies.main;
                main.startColor = Color.white;
            }
            else if (Vector2.Distance(transform.position, clicker.transform.position) <= 0.75f)
            {
                isWinning = true;
                score = "good";
                Debug.Log("good");
                clicker.transform.GetChild(1).GetComponent<TextMeshPro>().text = "GOOD";
                clicker.transform.GetChild(1).GetComponent<TextMeshPro>().color = new Color(1, 0.7f, 0.7f, 1);
                clicker.GetComponent<Animator>().ResetTrigger("hit");
                clicker.GetComponent<Animator>().SetTrigger("hit");
                GetComponent<SpriteRenderer>().color = new Color(1, 0.7f, 0.7f, 1);
                arah = (nurse.transform.position - transform.position).normalized;
                rb.velocity = arah * 30;
                trail.startColor = new Color(1, 0.7f, 0.7f, 1);
                ParticleSystem.MainModule main = dies.main;
                main.startColor = new Color(1, 0.7f, 0.7f, 1);
            }
            else if (Vector2.Distance(transform.position, clicker.transform.position) <= 1.2f)
            {
                isWinning = true;
                score = "bad";
                Debug.Log("bad");
                clicker.transform.GetChild(1).GetComponent<TextMeshPro>().text = "BAD";
                clicker.transform.GetChild(1).GetComponent<TextMeshPro>().color = Color.red;
                clicker.GetComponent<Animator>().ResetTrigger("hit");
                clicker.GetComponent<Animator>().SetTrigger("hit");
                GetComponent<SpriteRenderer>().color = new Color(1, 0.2f, 0.2f, 1);
                arah = (nurse.transform.position - transform.position).normalized;
                rb.velocity = arah * 30;
                trail.startColor = new Color(1, 0.2f, 0.2f, 1);
                ParticleSystem.MainModule main = dies.main;
                main.startColor = new Color(1, 0.2f, 0.2f, 1);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "healthBar")
        {
            isWinning = true;
            dies.Emit(50);
            gm.GetHit();
            nurse.Invoke("Attack", 1.5f);
            rb.velocity = Vector2.zero;
            transform.localScale = Vector3.zero;
            ParticleSystem.MainModule main = dies.main;
            main.startColor = new Color(1, 0.3f, 0.3f, 1);
            GetComponent<CircleCollider2D>().enabled = false;
            Invoke("Destroy", 2f);
        }
        else if(collision.tag == "Nurse")
        {
            isWinning = true;
            dies.Emit(50);
            nurse.getHit(score);
            rb.velocity = Vector2.zero;
            transform.localScale = Vector3.zero;
            ParticleSystem.MainModule main = dies.main;
            main.startColor = new Color(1, 1, 1, 1);
            GetComponent<CircleCollider2D>().enabled = false;
            Invoke("Destroy", 2f);
        }
        else if (collision.tag == "clicker")
        {
            fail = true;
            speed *= 100f;
            arah = (healthBar.transform.position - transform.position).normalized;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "clicker2")
        {
             canClick = false;
        }
    }
    private void Destroy()
    {
        Destroy(gameObject);
    }
}
