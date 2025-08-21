using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class john_gm : MonoBehaviour
{
    [SerializeField] private AudioSource main_bgm;
    [SerializeField] private AudioSource happy_bgm;
    [SerializeField] private Animator happyPanel;
    [SerializeField] private AudioSource happy_voice;
    [SerializeField] private Animator hintPanel;
    [SerializeField] private Volume volume;
    ShadowsMidtonesHighlights shadowsMidtonesHighlights;
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    Vector4 currentShadows;
    [SerializeField] private int health = 5;
    [Header("0 - 2")]
    [SerializeField] private int boss_index;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider healthBar_lerp;
    bool canHint = true;
    // Start is called before the first frame update
    void Start()
    {
        shake(0);
        virtualCam.m_Lens.OrthographicSize = 5f;
        volume.profile = Instantiate(volume.profile);
        if (volume.profile.TryGet<ShadowsMidtonesHighlights>(out shadowsMidtonesHighlights))
        {
            shadowsMidtonesHighlights.shadows.overrideState = true;
            currentShadows = shadowsMidtonesHighlights.shadows.value;
            shadowsMidtonesHighlights.shadows.value = new Vector4(0,0,0,0.3f);
        }
        health = PlayerPrefs.GetInt("health", 5);
    }

    // Update is called once per frame
    void Update()
    {
        //bar
        healthBar.value = health;
        healthBar_lerp.value = Mathf.MoveTowards(healthBar_lerp.value, health, Time.deltaTime * 2f);

        currentShadows = Vector4.Lerp(currentShadows, new Vector4(0,0,0,0.3f), Time.deltaTime * 0.7f);
        shadowsMidtonesHighlights.shadows.value = currentShadows;

        if(hintPanel.GetBool("in") && Input.GetMouseButtonDown(0) && canHint)
        {
            canHint = false;
            hintPanel.SetBool("in", false);
            CancelInvoke("change_canHint");
            Invoke("change_canHint", 0.1f);
        }
        if (happyPanel.GetBool("in"))
        {
            main_bgm.volume = Mathf.MoveTowards(main_bgm.volume, 0, Time.deltaTime * 1f);
            if (main_bgm.volume <= 0.001f)
            {
                happy_bgm.volume = Mathf.MoveTowards(happy_bgm.volume, 1f, Time.deltaTime * 1f);
            }
            if(!happy_voice.isPlaying && happy_bgm.volume >= 0.95f)
            {
                happyPanel.SetBool("in", false);
                Invoke("backToGameplay", 1f);
            }
        }
    }
    public void win()
    {
        PlayerPrefs.SetInt("boss_cleared" + boss_index, 10);
        happy_voice.Play();
        happyPanel.SetBool("in", true);
    }
    void backToGameplay()
    {
        FindObjectOfType<loading_screen>().startLoad("gameplay");
    }
    public void lose()
    {
        PlayerPrefs.SetInt("boss_cleared" + boss_index, 0);
        PlayerPrefs.SetInt("bossDoor" + boss_index, 120);
        FindObjectOfType<loading_screen>().startLoad("gameplay");
    }
    public void getHit()
    {
        GameObject.Find("Billy_hit").GetComponent<AudioSource>().Play();
        health--;
        if (health <= 0)
        {
            lose();
        }
        currentShadows = new Vector4(2.5f, 0, 0, 0f);
        PlayerPrefs.SetInt("health", health);
        shake(5);
    }
    public void shake(int value)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = value;
        CancelInvoke("stopShake");
        Invoke("stopShake", 0.5f);
    }
    void stopShake()
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }
    public void showHint()
    {
        if (canHint)
        {
            canHint = false;
            hintPanel.SetBool("in", !hintPanel.GetBool("in"));
        }
        CancelInvoke("change_canHint");
        Invoke("change_canHint", 0.1f);
    }
    void change_canHint()
    {
        canHint = true;
    }
}
