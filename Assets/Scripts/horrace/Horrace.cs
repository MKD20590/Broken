using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Horrace : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites;
    [SerializeField] private List<Material> materials;
    [Header("ada 24, random posisi (index 3 terakhir selalu bener)")]
    [SerializeField] private AudioSource Horrace_voice;
    [SerializeField] private AudioSource Billy_voice;
    [SerializeField] private List<AudioClip> Horrace_voice_clip;
    [SerializeField] private List<AudioClip> Billy_voice_clip;
    [SerializeField] private List<string> word1;
    [SerializeField] private List<string> word2;
    [SerializeField] private List<string> word3;
/*    [Header("ada 3, random posisi")]
    [SerializeField] private List<string> word_baik1;
    [SerializeField] private List<string> word_baik2;
    [SerializeField] private List<string> word_baik3;*/
    List<int> wordIndexes = new List<int> {1,2,3};
    int wordIdx = 0;
    [Header("ada 24")]
    [SerializeField] private List<horrace_word> word_position;
    [SerializeField] private List<float> timer;
    [SerializeField] private TextMeshProUGUI timerText;
    horrace_gm gm;
    int stage = 1;
    int hp = 3;
    bool timerRunning = true;
    bool billylaying = false;
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        foreach (horrace_word word in FindObjectsOfType<horrace_word>())
        {
            word_position.Add(word);
        }
        gm = FindObjectOfType<horrace_gm>();
        StartCoroutine(spawnWord());
    }

    // Update is called once per frame
    void Update()
    {
        if (timer[stage-1] > 0 && timerRunning)
        {
            timer[stage-1] -= Time.deltaTime;
        }
        else if (timerRunning)
        {
            timerRunning = false;
            for (int i = 0; i < word_position.Count; i++)
            {
                word_position[i].explode(true);
            }
            gm.getHit();
            Invoke("nextStage", 0.5f);
        }
        int menit = Mathf.FloorToInt(timer[stage - 1] / 60);
        int detik = Mathf.FloorToInt(timer[stage - 1] % 60);
        timerText.color = Color.Lerp(timerText.color, Color.white, Time.deltaTime * 2f);
        timerText.text = string.Format("{0:00}:{1:00}", menit, detik);

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

        if(billylaying && !Billy_voice.isPlaying)
        {
            billylaying = false;
            nextStage();
        }
    }
    public void minusTimer()
    {
        if (timer[stage - 1] > 0 && timerRunning)
        {
            timer[stage - 1] -= 10f;
            if (timer[stage - 1] < 0)
            {
                timer[stage - 1] = 0;
            }
            timerText.color = Color.red;
        }
    }
    public void nextStage()
    {
        if (stage < 3)
        {
            stage++;
            StartCoroutine(spawnWord());
            timerRunning = true;
            billylaying = false;
        }
        else
        {
            if (hp > 0)
            {
                gm.lose();
            }
            else
            {
                gm.win();
            }
        }
    }
    public void getHit()
    {
        gm.shake(5);
        GameObject.Find("Monster_hit").GetComponent<AudioSource>().Play();
        foreach (SpriteRenderer sr in sprites)
        {
            sr.color = Color.red;
        }
        hp--;
        for (int i = 0; i < word_position.Count; i++)
        {
            word_position[i].explode(false);
        }
        anim.ResetTrigger("hit");
        anim.SetTrigger("hit");
        Billy_voice.Stop();
        Billy_voice.clip = Billy_voice_clip[wordIdx - 1];
        Billy_voice.Play();
        timerRunning = false;
        billylaying = true;
        //Invoke("nextStage", 0.5f);
    }
    public IEnumerator spawnWord()
    {
        yield return new WaitForSeconds(.5f);
        wordIdx = wordIndexes[Random.Range(0, wordIndexes.Count)];
        wordIndexes.Remove(wordIdx);

        Horrace_voice.Stop();
        Horrace_voice.clip = Horrace_voice_clip[wordIdx - 1];
        Horrace_voice.Play();

        List<int> availableIdx = new List<int>();
        List<int> availablePos = new List<int>();
        for(int i = 0; i < word_position.Count; i++)
        {
            availableIdx.Add(i);
            availablePos.Add(i);
        }
        if(wordIdx == 1)
        {
            for(int i = 0; i < word_position.Count; i++)
            {                
                int idxPos = availablePos[Random.Range(0, availablePos.Count)];
                int idx = availableIdx[Random.Range(0, availableIdx.Count)];
                StartCoroutine(word_position[idxPos].setWord(word1[idx]));
                if (idx == word1.Count - 1 ||
                    idx == word1.Count - 2 ||
                    idx == word1.Count - 3)
                {
                    word_position[idxPos].baik = true;
                }
                availableIdx.Remove(idx);
                availablePos.Remove(idxPos);
                yield return new WaitForSeconds(0.02f);
            }
        }
        else if(wordIdx == 2)
        {
            for (int i = 0; i < word_position.Count; i++)
            {
                int idxPos = availablePos[Random.Range(0, availablePos.Count)];
                int idx = availableIdx[Random.Range(0, availableIdx.Count)];
                StartCoroutine(word_position[idxPos].setWord(word2[idx]));
                if (idx == word1.Count - 1 ||
                    idx == word1.Count - 2 ||
                    idx == word1.Count - 3)
                {
                    word_position[idxPos].baik = true;
                }
                availableIdx.Remove(idx);
                availablePos.Remove(idxPos);
                yield return new WaitForSeconds(0.02f);
            }
        }
        else
        {
            for (int i = 0; i < word_position.Count; i++)
            {
                int idxPos = availablePos[Random.Range(0, availablePos.Count)];
                int idx = availableIdx[Random.Range(0, availableIdx.Count)];
                StartCoroutine(word_position[idxPos].setWord(word3[idx]));
                if (idx == word1.Count - 1 ||
                    idx == word1.Count - 2 ||
                    idx == word1.Count - 3)
                {
                    word_position[idxPos].baik = true;
                }
                availableIdx.Remove(idx);
                availablePos.Remove(idxPos);
                yield return new WaitForSeconds(0.02f);
            }
        }
    }
}
