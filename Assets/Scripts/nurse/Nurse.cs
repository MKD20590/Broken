using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nurse : MonoBehaviour
{
    [SerializeField] private Animator syringeAnim;
    [SerializeField] private List<SpriteRenderer> sprites;
    [SerializeField] private List<Material> materials;
    [SerializeField] private AudioSource Nurse_voice;
    [SerializeField] private AudioSource Billy_voice;
    [SerializeField] private List<AudioClip> Nurse_voice_clip;
    [SerializeField] private List<AudioClip> Billy_voice_clip;
    int voiceIdx = 0;
    Animator anim;
    nurse_gm gm;
    [SerializeField] private nurse_syringe syringe;
    [SerializeField] private float hp = 4f;
    int count = 0;
    bool hitDone = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        gm = FindObjectOfType<nurse_gm>();
        syringe = FindObjectOfType<nurse_syringe>();
        Invoke("attack", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Material mat in materials)
        {
            if (hp <= 4 && hp > 3)
            {
                mat.SetFloat("_glitch_value", 0);
                mat.SetFloat("_noise_scale", 0);
                mat.SetFloat("_multiply", 0);
                mat.SetFloat("_alpha_value", 1f);
            }
            else if (hp <= 3 && hp > 2)
            {
                mat.SetFloat("_glitch_value", UnityEngine.Random.Range(3, 5));
                mat.SetFloat("_noise_scale", UnityEngine.Random.Range(30, 70));
                mat.SetFloat("_multiply", 1f);
                mat.SetFloat("_alpha_value", UnityEngine.Random.Range(0.7f, 0.9f));
            }
            else if (hp <= 2 && hp > 1)
            {
                mat.SetFloat("_glitch_value", UnityEngine.Random.Range(5, 7));
                mat.SetFloat("_noise_scale", UnityEngine.Random.Range(70, 120));
                mat.SetFloat("_multiply", 3f);
                mat.SetFloat("_alpha_value", UnityEngine.Random.Range(0.5f, 0.7f));
            }
            else if(hp <= 1 && hp > 0)
            {
                mat.SetFloat("_glitch_value", UnityEngine.Random.Range(7, 10));
                mat.SetFloat("_noise_scale", UnityEngine.Random.Range(120, 150));
                mat.SetFloat("_multiply", 5f);
                mat.SetFloat("_alpha_value", UnityEngine.Random.Range(0f, 0.5f));
            }
            else
            {
                mat.SetFloat("_glitch_value", UnityEngine.Random.Range(10, 15));
                mat.SetFloat("_noise_scale", UnityEngine.Random.Range(150, 170));
                mat.SetFloat("_multiply", 7f);
                mat.SetFloat("_alpha_value", UnityEngine.Random.Range(0f, 0.2f));
            }
        }
        if (hp > 0)
        {
            foreach (SpriteRenderer sr in sprites)
            {
                sr.color = Color.Lerp(sr.color, new Color(1, 1, 1, 1), Time.deltaTime * 3f);
            }
        }
        else
        {
            foreach (SpriteRenderer sr in sprites)
            {
                sr.color = Color.Lerp(sr.color, new Color(1, 1, 1, 0), Time.deltaTime * 1f);
            }
        }
        if(hitDone && !Billy_voice.isPlaying)
        {
            hitDone = false;
            attack();
        }
    }
    public void getHit(string score)
    {
        GameObject.Find("Monster_hit").GetComponent<AudioSource>().Play();
        if (score == "perfect")
        {
            hp -= 1f;
        }
        else if (score == "good")
        {
            hp-= 0.8f;
        }
        else if (score == "bad")
        {
            hp-= 0.6f;
        }
        if(hp < 0)
        {
            hp = 0;
            syringeAnim.SetBool("dead", true);
        }
        gm.shake(5);
        anim.ResetTrigger("hit");
        anim.SetTrigger("hit");
        Billy_voice.Stop();
        Billy_voice.clip = Billy_voice_clip[voiceIdx];
        Billy_voice.Play();
        Billy_voice_clip.RemoveAt(voiceIdx);
        hitDone = true;
    }
    public void attack()
    {
        //hp = Mathf.Round(hp);
        if(hp > 0 && count < 5)
        {
            count++;
            voiceIdx = Random.Range(0, Nurse_voice_clip.Count);
            Nurse_voice.Stop();
            Nurse_voice.clip = Nurse_voice_clip[voiceIdx];
            Nurse_voice.Play();
            Nurse_voice_clip.RemoveAt(voiceIdx);
            syringe.attack();
        }
        else if(hp <= 0)
        {
            gm.win();
        }
        else if(count >= 5)
        {
            gm.lose();
        }
    }
}
