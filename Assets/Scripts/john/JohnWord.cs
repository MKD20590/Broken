using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnWord : MonoBehaviour
{
    JohnGM gm;
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
        gm = FindObjectOfType<JohnGM>();
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
            Destroy();
        }
    }
    public void CheckAnswer(int idx)
    {
        if (john.current_answer_kalimat.answer_kalimat[idx] != correct_answer)
        {
            anim.SetTrigger("attack");
            Debug.Log("wrong answer");
            isAttacking = true;
            isDestroying = true;
        }
        else
        {
            john.BillyTalk();
            john.GetHit();
            Debug.Log("correct answer");
            anim.SetTrigger("die");
            Die(false);
        }
    }
    public void Attack()
    {
        Vector2 target = (healthbar.transform.position - transform.position).normalized;
        rb.AddForce(target*700f, ForceMode2D.Impulse);
    }
    public void Die(bool attack)
    {
        john.ResetAnswer();
        rb.velocity = Vector2.zero;
        anim.SetTrigger("die");
        bg.Stop();
        dies.Emit(50);
        if (!attack)
        {
            gm.Shake(3);
        }
        if(isDestroying) Invoke("Destroy", 2f);
        else
        {
            isDestroying = true;
        }
    }
    public void Destroy()
    {
        john.SpawnWord();
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "healthBar")
        {
            GetComponent<BoxCollider2D>().enabled = false;
            //Debug.Log("hit healthbar");
            gm.GetHit();
            Die(true);
        }
    }
}
