using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class John : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites;
    [SerializeField] private List<Material> materials;
    [SerializeField] private List<GameObject> wordsPrefabs;
    [SerializeField] private List<AudioClip> words_voice;
    [SerializeField] private AudioSource john_voice;
    [SerializeField] private AudioSource billy_voice;
    public int hp = 3;

    [Serializable]
    public class answers
    {
        public string current_words;
        public List<string> answer_kalimat;
        public AudioClip answer_voice;
    }
    [SerializeField] private List<answers> answer_kalimat;
    public answers current_answer_kalimat;
    [SerializeField] private List<john_answer> answer_Text;
    Animator anim;
    public john_word word;
    john_gm gm;
    int totalWords = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Material mat in materials)
        {
            mat.SetFloat("_glitch_value", 0);
            mat.SetFloat("_noise_scale", 0);
            mat.SetFloat("_multiply", 0);
            mat.SetFloat("_alpha_value", 1f);
        }
        gm = FindObjectOfType<john_gm>();
        anim = GetComponent<Animator>();
        Invoke("spawnWord", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Material mat in materials)
        {
            if (hp == 3)
            {
                mat.SetFloat("_glitch_value", 0);
                mat.SetFloat("_noise_scale", 0);
                mat.SetFloat("_multiply", 0);
                mat.SetFloat("_alpha_value", 1f);
            }
            else if (hp == 2)
            {
                mat.SetFloat("_glitch_value", UnityEngine.Random.Range(3, 5));
                mat.SetFloat("_noise_scale", UnityEngine.Random.Range(30, 70));
                mat.SetFloat("_multiply", 1f);
                mat.SetFloat("_alpha_value", UnityEngine.Random.Range(0.7f, 0.9f));
            }
            else if (hp == 1)
            {
                mat.SetFloat("_glitch_value", UnityEngine.Random.Range(5, 7));
                mat.SetFloat("_noise_scale", UnityEngine.Random.Range(70, 120));
                mat.SetFloat("_multiply", 3f);
                mat.SetFloat("_alpha_value", UnityEngine.Random.Range(0.5f, 0.7f));
            }
            else
            {
                mat.SetFloat("_glitch_value", UnityEngine.Random.Range(7, 10));
                mat.SetFloat("_noise_scale", UnityEngine.Random.Range(120, 150));
                mat.SetFloat("_multiply", 5f);
                mat.SetFloat("_alpha_value", UnityEngine.Random.Range(0f, 0.5f));
            }
        }
        if (hp > 0)
        {
            foreach (SpriteRenderer sr in sprites)
            {
                sr.color = Color.Lerp(sr.color, new Color(1,1,1,1), Time.deltaTime * 3f);
            }
        }
        else
        {
            foreach (SpriteRenderer sr in sprites)
            {
                sr.color = Color.Lerp(sr.color, new Color(1, 1, 1, 0), Time.deltaTime * 1f);
            }
        }
    }
    public void spawnWord()
    {
        if (totalWords < 3)
        {
            totalWords++;
            int idx = UnityEngine.Random.Range(0, wordsPrefabs.Count);
            word = Instantiate(wordsPrefabs[idx], transform.position, Quaternion.identity).GetComponent<john_word>();
            current_answer_kalimat = answer_kalimat[answer_kalimat.FindIndex(x => x.current_words == word.current_words)];
            addAnswer();
            john_voice.Stop();
            john_voice.clip = words_voice[idx];
            john_voice.Play();
            wordsPrefabs.RemoveAt(idx);
            words_voice.RemoveAt(idx);
        }
        else
        {
            if (hp <= 0)
            {
                gm.win();
            }
            else
            {
                gm.lose();
            }
        }
    }
    public void getHit()
    {
        GameObject.Find("Monster_hit").GetComponent<AudioSource>().Play();
        gm.shake(5);
        foreach (SpriteRenderer sr in sprites)
        {
            sr.color = Color.red;
        }
        hp--;
        anim.ResetTrigger("hit");
        anim.SetTrigger("hit");
    }
    void addAnswer()
    {
        List<int> availableIdx = new List<int>{0, 1, 2};
        for (int i = 0; i < answer_Text.Count; i++)
        {
            int index = availableIdx[UnityEngine.Random.Range(0, availableIdx.Count)];
            answer_Text[i].current_answer = current_answer_kalimat.answer_kalimat[index];
            StartCoroutine(answer_Text[i].randomize());
            availableIdx.Remove(index);
        }
    }
    public void checkAnswer(int idx)
    {
        if (word != null)
        {
            int index = current_answer_kalimat.answer_kalimat.FindIndex(x => x == answer_Text[idx].current_answer);
            word.checkAnswer(index);
        }
    }
    public void billyTalk()
    {
        billy_voice.Stop();
        billy_voice.clip = current_answer_kalimat.answer_voice;
        billy_voice.Play();
    }
    public void resetAnswer()
    {
        foreach (john_answer answer in answer_Text)
        {
            StartCoroutine(answer.deleteText());
        }
    }
}
