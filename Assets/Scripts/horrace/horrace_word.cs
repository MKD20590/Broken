using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class horrace_word : MonoBehaviour
{
    public string word;
    public bool baik = false;
    horrace_gm gm;
    Horrace horrace;
    LineRenderer lineRenderer;
    [SerializeField] private ParticleSystem bg;
    [SerializeField] private ParticleSystem dies;
    public static bool isHolding = false;
    public static int curr_lineIdx = 0;
    [SerializeField] private TextMeshPro text;
    public static List<GameObject> connected = new List<GameObject>();
    Animator anim;
    float timer = 60f;
    CapsuleCollider2D coll;
    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<CapsuleCollider2D>();
        horrace = FindObjectOfType<Horrace>();
        anim = GetComponent<Animator>();
        lineRenderer = GameObject.Find("Line_Word").GetComponent<LineRenderer>();
        gm = FindObjectOfType<horrace_gm>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(curr_lineIdx);
        if (text.text != "")
        {
            text.color = Color.Lerp(text.color, new Color(1, 1, 1, 1), Time.deltaTime * 2f);
        }
        else
        {
            text.color = Color.Lerp(text.color, new Color(0, 0, 0, 0), Time.deltaTime * 2f);
        }
        if (!baik)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = 60f;
                blink();
            }
        }
        if(isHolding && connected[0] == gameObject)
        {
            lineRenderer.SetPosition(curr_lineIdx, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0));
            if(connected.Count >= 3)
            {
                lineRenderer.startColor = new Color(0, 0, 0, 0);
                lineRenderer.endColor = new Color(0, 0, 0, 0);
                curr_lineIdx = 0;
                gm.shake(3);
                horrace.getHit();
                isHolding = false;
                connected.Clear();
            }
        }
        else if(!isHolding)
        {
            if(lineRenderer.positionCount >= 2)
            {
                if(Vector2.Distance(lineRenderer.GetPosition(lineRenderer.positionCount-1), lineRenderer.GetPosition(lineRenderer.positionCount-2)) > 0.07f)
                {
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, Vector2.MoveTowards(lineRenderer.GetPosition(lineRenderer.positionCount - 1), lineRenderer.GetPosition(lineRenderer.positionCount - 2), Time.deltaTime * 1f));
                }
                else
                {
                    if(lineRenderer.positionCount >= 2) lineRenderer.positionCount--;
                }
            }
        }
    }
    void blink()
    {
        text.color = new Color(1, 0, 0, 1);
    }
    private void OnMouseEnter()
    {
        if (isHolding)
        {            
            if (!baik)
            {
                lineRenderer.startColor = new Color(1, 0, 0, 0);
                lineRenderer.endColor = new Color(1, 0, 0, 1);
                curr_lineIdx = 0;
                gm.shake(3);
                horrace.minusTimer();
                isHolding = false;
                connected.Clear();
            }
            else
            {
                if(connected.Count <= 3 && !connected.Contains(gameObject))
                {
                    lineRenderer.positionCount++;
                    connected.Add(gameObject);
                    lineRenderer.SetPosition(curr_lineIdx, transform.position);
                    curr_lineIdx++;
                }
            }
        }
    }
    private void OnMouseDown()
    {
        if(baik)
        {
            lineRenderer.startColor = new Color(1, 1, 1, 0);
            lineRenderer.endColor = new Color(1, 1, 1, 1);
            curr_lineIdx++;
            lineRenderer.positionCount = 2;
            isHolding = true;
            lineRenderer.SetPosition(0, transform.position);
            connected.Add(gameObject);
        }
        else
        {
            blink();
            gm.shake(3);
            horrace.minusTimer();
            connected.Clear();
        }
    }
    private void OnMouseUp()
    {
        isHolding = false;
        curr_lineIdx = 0;
        connected.Clear();
    }
    public IEnumerator setWord(string word)
    {
        bg.Play();
        anim.SetBool("in",true);
        for(int i=0;i<word.Length;i++)
        {
            text.text += word[i];
            yield return new WaitForSeconds(0.05f);
        }
        coll.enabled = true;
    }
    public void explode(bool attacking)
    {
        if(attacking)
        {
            ParticleSystem.MainModule main = dies.main;
            main.startColor = new Color(1, 0.3f, 0.3f, 1);
        }
        else
        {
            ParticleSystem.MainModule main = dies.main;
            main.startColor = new Color(1, 1, 1, 1);
        }
        coll.enabled = false;
        dies.Emit(50);
        text.text = "";
        wordOut();
        baik = false;
    }
    public void wordOut()
    {
        coll.enabled = false;
        bg.Stop();
        anim.SetBool("in", false);
    }
}
