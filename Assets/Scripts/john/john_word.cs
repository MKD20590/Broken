using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class john_word : MonoBehaviour
{
    john_gm gm;
    John john;
    [SerializeField] private ParticleSystem bg;
    [SerializeField] private ParticleSystem dies;
    [SerializeField] private GameObject healthbar;
    public string current_words;
    public string correct_answer;
    Rigidbody2D rb;
    Animator anim;
    int totalChecks = 1;
    bool isAttacking = false;
    bool isDestroying = false;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<john_gm>();
        john = FindObjectOfType<John>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(Random.Range(1,2.3f), Random.Range(-1f, 1.5f)),ForceMode2D.Impulse);
        healthbar = GameObject.Find("HP_bar");
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAttacking && isDestroying && !GameObject.Find("Billy_Voice").GetComponent<AudioSource>().isPlaying)
        {
            isDestroying = false;
            destroy();
        }
    }
    public void checkAnswer(int idx)
    {
        if (john.current_answer_kalimat.answer_kalimat[idx] != correct_answer)
        {
/*            if (totalChecks < 3)
            {
                totalChecks++;
            }
            else
            {
                anim.SetTrigger("attack");
            }*/
            anim.SetTrigger("attack");
            Debug.Log("wrong answer");
            isAttacking = true;
            isDestroying = true;
            //anim.SetFloat("speed",anim.GetFloat("speed")+1f);
        }
        else
        {
            john.billyTalk();
            john.getHit();
            Debug.Log("correct answer");
            anim.SetTrigger("die");
            die(false);
        }
    }
    public void attack()
    {
        Vector2 target = (healthbar.transform.position - transform.position).normalized;
        rb.AddForce(target*700f, ForceMode2D.Impulse);
    }
    public void die(bool attack)
    {
        john.resetAnswer();
        rb.velocity = Vector2.zero;
        anim.SetTrigger("die");
        bg.Stop();
        dies.Emit(50);
        if (!attack)
        {
            gm.shake(3);
        }
        if(isDestroying) Invoke("destroy", 2f);
        else
        {
            isDestroying = true;
        }
    }
    public void destroy()
    {
        john.spawnWord();
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "healthBar")
        {
            GetComponent<BoxCollider2D>().enabled = false;
            //Debug.Log("hit healthbar");
            gm.getHit();
            die(true);
        }
    }
}
