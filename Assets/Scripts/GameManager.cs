using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Animator hintPanel;
    [SerializeField] private List<Image> totalHints;
    [SerializeField] private List<string> hints;
    [SerializeField] private List<char> acak;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private int currentHint = 0;

    Player player;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider sanityBar;
    [SerializeField] private Slider healthBar_lerp;
    [SerializeField] private Slider sanityBar_lerp;
    [SerializeField] private Animator pausePanel;
    [SerializeField] private GameObject glimpses;
    [SerializeField] private List<Material> glimpses_mats;

    [SerializeField] private Slider bgm;
    [SerializeField] private Slider sfx;
    [SerializeField] private AudioMixer mixer;

    [SerializeField] private int minTime_glimpses;
    [SerializeField] private int maxTime_glimpses;

    static bool isHinting = true;

    int random_glimpse = 0;
    float time_glimpse = 0;
    bool pause = false;
    [SerializeField] private bool canHint = true;
    [SerializeField] private bool canHintClick = true;
    // Start is called before the first frame update
    void Awake()
    {
        hintPanel.SetBool("in", isHinting);
        acak = hintText.text.ToCharArray().ToList();
        Time.timeScale = 1;
        random_glimpse = Random.Range(minTime_glimpses, maxTime_glimpses);
        player = FindObjectOfType<Player>();
        if (!PlayerPrefs.HasKey("bgm") || !PlayerPrefs.HasKey("sfx"))
        {
            PlayerPrefs.SetFloat("bgm", 1f);
            PlayerPrefs.SetFloat("sfx", 1f);
            bgm.value = 1;
            sfx.value = 1;
            mixer.SetFloat("bgm", PlayerPrefs.GetFloat("bgm"));
            mixer.SetFloat("sfx", PlayerPrefs.GetFloat("sfx"));
        }
        else
        {
            bgm.value = PlayerPrefs.GetFloat("bgm");
            sfx.value = PlayerPrefs.GetFloat("sfx");
            mixer.SetFloat("bgm", PlayerPrefs.GetFloat("bgm"));
            mixer.SetFloat("sfx", PlayerPrefs.GetFloat("sfx"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        //audio
        if (PlayerPrefs.HasKey("bgm") && PlayerPrefs.HasKey("sfx"))
        {
            PlayerPrefs.SetFloat("bgm", bgm.value);
            PlayerPrefs.SetFloat("sfx", sfx.value);
            mixer.SetFloat("bgm", Mathf.Log10(PlayerPrefs.GetFloat("bgm")) * 20);
            mixer.SetFloat("sfx", Mathf.Log10(PlayerPrefs.GetFloat("sfx")) * 20);
        }
        else
        {
            mixer.SetFloat("bgm", Mathf.Log10(bgm.value) * 20);
            mixer.SetFloat("sfx", Mathf.Log10(sfx.value) * 20);
        }

        //bar
        healthBar.value = player.health;
        sanityBar.value = player.sanity;

        healthBar_lerp.value = Mathf.MoveTowards(healthBar_lerp.value, player.health, Time.deltaTime * 5f);
        sanityBar_lerp.value = Mathf.MoveTowards(sanityBar_lerp.value, player.sanity, Time.deltaTime * 5f);

        //glimpses
        if (time_glimpse >= random_glimpse)
        {
            glimpses.GetComponent<Image>().material.SetFloat("_alpha", Mathf.MoveTowards(glimpses.GetComponent<Image>().material.GetFloat("_alpha"), .95f, Time.deltaTime * 10f));
            if (glimpses.GetComponent<Image>().material.GetFloat("_alpha") >= .95f)
            {
                random_glimpse = Random.Range(minTime_glimpses, maxTime_glimpses);
                time_glimpse = 0;
            }
        }
        else
        {
            time_glimpse += Time.deltaTime;
            glimpses.GetComponent<Image>().material.SetFloat("_alpha", Mathf.MoveTowards(glimpses.GetComponent<Image>().material.GetFloat("_alpha"), 0, Time.deltaTime * 2f));
            if (glimpses.GetComponent<Image>().material.GetFloat("_alpha") <= 0f)
            {
                glimpses.GetComponent<Image>().material = glimpses_mats[Random.Range(0,glimpses_mats.Count)];

            }
        }

        foreach (Image h in totalHints)
        {
            if(totalHints.IndexOf(h) == currentHint)
            {
                h.color = Color.Lerp(h.color, new Color(1f, 1f, 1f, 1f), Time.deltaTime * 5f);
            }
            else
            {
                h.color = Color.Lerp(h.color, new Color(0.3f, 0.3f, 0.3f, 1f), Time.deltaTime * 5f);
            }
        }

        //pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Paused();
        }
    }
    public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("warned", 10);
        PlayerPrefs.SetInt("health", 5);
        PlayerPrefs.SetInt("sanity", 0);
        FindObjectOfType<loading_screen>().startLoad("gameplay");
    }
    public void BackMenu()
    {
        FindObjectOfType<loading_screen>().startLoad("main_menu");
    }
    public void ShowHint()
    {
        if (canHint)
        {
            canHintClick = true;
            isHinting = !isHinting;
            canHint = false;
            hintPanel.SetBool("in", !hintPanel.GetBool("in"));
        }
        CancelInvoke("ChangeCanHint");
        Invoke("ChangeCanHint", 0.1f);
    }
    public void Hover()
    {
        canHintClick = false;
    }
    public void Unhover()
    {
        canHintClick = true;
    }
    void ChangeCanHint()
    {
        canHint = true;
    }
    public void NextHint()
    {
        if(canHint)
        {
            canHint = false;
            if (currentHint < hints.Count - 1)
            {
                currentHint++;
            }
            else
            {
                currentHint = 0;
            }
            StartCoroutine(ChangeHint());
        }
    }
    public void PrevHint()
    {
        if(canHint)
        {
            canHint = false;
            if (currentHint > 0)
            {
                currentHint--;
            }
            else
            {
                currentHint = hints.Count - 1;
            }
            StartCoroutine(ChangeHint());
        }
    }
    IEnumerator ChangeHint()
    {
        int randomizeCount = acak.Count-1;
        while (randomizeCount >= 0)
        {
            for (int i = acak.Count - 1; i > randomizeCount - 1; i--)
            {
                int idx = Random.Range('a', 'z');
                acak[i] = (char)idx;
            }
            randomizeCount--;
            hintText.text = new string(acak.ToArray());
            yield return new WaitForSeconds(0.005f);
        }
        while (randomizeCount < hints[currentHint].Length && acak.Count > 0)
        {
            for (int i = 0; i < acak.Count; i++)
            {
                int idx = Random.Range('a', 'z');
                acak[i] = (char)idx;
            }

            acak.RemoveAt(acak.Count - 1);
            hintText.text = new string(acak.ToArray());
            yield return new WaitForSeconds(0.005f);
        }
        randomizeCount = 0;
        for (int i = acak.Count-1; i < hints[currentHint].Length; i++)
        {
            int idx = Random.Range('a', 'z');
            yield return new WaitForSeconds(0.01f);
            for (int j = 0; j < acak.Count; j++)
            {
                int index = Random.Range('a', 'z');
                acak[j] = (char)index;
            }
            if(acak.Count < hints[currentHint].Length)acak.Add((char)idx);
            hintText.text = new string(acak.ToArray());
        }
        char[] temp = hints[currentHint].ToCharArray();
        while (randomizeCount < temp.Length)
        {
            acak[randomizeCount] = temp[randomizeCount];
            yield return new WaitForSeconds(0.01f);
            for (int i = randomizeCount + 1; i < acak.Count; i++)
            {
                int idx = Random.Range('a', 'z');
                acak[i] = (char)idx;
            }
            hintText.text = new string(acak.ToArray());
            randomizeCount++;
        }
        yield return new WaitForSeconds(0.5f);
        canHint = true;
    }
    public void Paused()
    {
        pause = !pause;
        if (pause)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        pausePanel.SetBool("in", pause);
    }
}
