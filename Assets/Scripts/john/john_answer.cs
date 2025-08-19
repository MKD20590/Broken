using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class john_answer : MonoBehaviour
{
    John john;
    john_gm gm;
    //Animator anim;
    [SerializeField] private TextMeshProUGUI answer_Text;
    [SerializeField] private ParticleSystem bg;
    [SerializeField] private ParticleSystem die;
    public string current_answer = "";
    public List<char> acak;
    public bool canAnswer = false;
    public static bool canAnswer2 = false;
    bool isAnswering = false;
    int randomizeCount = 0;
    [SerializeField] private int indexAnswer;
    public bool isCorrect = false;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<john_gm>();
        //anim = GetComponent<Animator>();
        john = FindObjectOfType<John>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isAnswering && Vector2.Distance(answer_Text.transform.position, john.word.transform.position) > 0.01f)
        {
            answer_Text.transform.position = Vector2.MoveTowards(answer_Text.transform.position, john.word.transform.position, Time.deltaTime * 30f);
        }
        else if(isAnswering && Vector2.Distance(answer_Text.transform.position, john.word.transform.position) < 0.01f)
        {
            bg.Clear();
            gm.shake(3);
            acak.Clear();
            john.checkAnswer(indexAnswer);
            isAnswering = false;
            answer_Text.text = "";
            die.Emit(50);
            answer_Text.transform.localPosition = Vector2.zero;
        }
        if(!canAnswer2/* && !GameObject.Find("John_Voice").GetComponent<AudioSource>().isPlaying*/)
        {
            foreach(john_answer a in FindObjectsOfType<john_answer>())
            {
                if(!a.canAnswer)
                {
                    canAnswer2 = false;
                    return;
                }
            }
            canAnswer2 = true;
        }
    }
    public void answer()
    {
        if (canAnswer && canAnswer2)
        {
            if(!isCorrect)
            {
                bg.Clear();
                ParticleSystem.MainModule main = bg.main;
                main.startColor = new Color(1, 0, 0, 1);
                bg.Emit(30);
            }
            bg.Stop();
            isAnswering = true;
            canAnswer = false;
            canAnswer2 = false;
            //anim.ResetTrigger("answer");
            //anim.SetTrigger("answer");
        }
    }
    public IEnumerator randomize()
    {
        ParticleSystem.MainModule main = bg.main;
        main.startColor = new Color(1, 1, 1, 1);
        randomizeCount = 0;
        bg.Play();
        isAnswering = false;
        acak.Clear();
        //acak = current_answer.ToCharArray();
        for (int i = 0; i < current_answer.Length; i++)
        {
            int idx = Random.Range('a','z');
            yield return new WaitForSeconds(0.03f);
            for (int j = 0; j < acak.Count; j++)
            {
                int index = Random.Range('a', 'z');
                acak[j] = (char)index;
            }
            acak.Add((char)idx);
            answer_Text.text = new string(acak.ToArray());
        }
        char[] temp = current_answer.ToCharArray();
        while (randomizeCount < temp.Length)
        {
            acak[randomizeCount] = temp[randomizeCount];
            yield return new WaitForSeconds(0.03f);
            for (int i = randomizeCount + 1; i < acak.Count; i++)
            {
                int idx = Random.Range('a', 'z');
                acak[i] = (char)idx;
            }
            answer_Text.text = new string(acak.ToArray());
            randomizeCount++;
        }
        randomizeCount = 0;
        canAnswer = true;
    }
    public IEnumerator deleteText()
    {
        isCorrect = false;
        bg.Stop();
        current_answer = "";
        canAnswer = false;
        isAnswering = false;
        randomizeCount = acak.Count-1;
        while (randomizeCount >= 0)
        {
            for (int i = acak.Count-1; i > randomizeCount-1; i--)
            {
                int idx = Random.Range('a', 'z');
                acak[i] = (char)idx;
            }
            randomizeCount--;
            answer_Text.text = new string(acak.ToArray());
            yield return new WaitForSeconds(0.02f);
        }
        while(randomizeCount < 0 && acak.Count > 0)
        {
            for (int i = 0; i < acak.Count; i++)
            {
                int idx = Random.Range('a', 'z');
                acak[i] = (char)idx;
            }

            acak.RemoveAt(acak.Count-1);
            answer_Text.text = new string(acak.ToArray());
            yield return new WaitForSeconds(0.02f);
        }
        randomizeCount = 0;
    }
}
