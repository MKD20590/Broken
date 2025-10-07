using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class horrace_gm : MonoBehaviour, IMonsterManager
{
    [SerializeField] private AudioSource main_bgm;
    [SerializeField] private AudioSource happy_bgm;
    [SerializeField] private Animator happyPanel;
    [SerializeField] private AudioSource happy_voice1;
    [SerializeField] private AudioSource happy_voice2;
    int happyPlayed = 0;
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
    bool isWinning = false;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        Shake(0);
        virtualCam.m_Lens.OrthographicSize = 5f;
        volume.profile = Instantiate(volume.profile);
        if (volume.profile.TryGet<ShadowsMidtonesHighlights>(out shadowsMidtonesHighlights))
        {
            shadowsMidtonesHighlights.shadows.overrideState = true;
            currentShadows = shadowsMidtonesHighlights.shadows.value;
            shadowsMidtonesHighlights.shadows.value = new Vector4(0, 0, 0, 0.3f);
        }
        health = PlayerPrefs.GetInt("health", 5);
    }

    // Update is called once per frame
    void Update()
    {
        //bar
        healthBar.value = health;
        healthBar_lerp.value = Mathf.MoveTowards(healthBar_lerp.value, health, Time.deltaTime * 2f);

        currentShadows = Vector4.Lerp(currentShadows, new Vector4(0, 0, 0, 0.3f), Time.deltaTime * 0.7f);
        shadowsMidtonesHighlights.shadows.value = currentShadows;

        if (hintPanel.GetBool("in"))
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        if (happyPanel.GetBool("in"))
        {
            main_bgm.volume = Mathf.MoveTowards(main_bgm.volume, 0, Time.deltaTime * 1f);
            if(main_bgm.volume <= 0.001f)
            {
                happy_bgm.volume = Mathf.MoveTowards(happy_bgm.volume, 0.85f, Time.deltaTime * 1f);
            }
            if (!happy_voice1.isPlaying && happyPlayed == 1)
            {
                happy_voice2.Play();
                happyPlayed++;
            }
            else if (!happy_voice2.isPlaying && happyPlayed == 2 && happy_bgm.volume >= 0.8f)
            {
                happyPlayed = 0;
                happyPanel.SetBool("in", false);
                Invoke("BackToGameplay", 1f);
            }
        }
    }
    public void Win()
    {
        isWinning = true;
        hintPanel.SetBool("in", false);
        PlayerPrefs.SetInt("boss_cleared" + boss_index, 10);
        happy_voice1.Play();
        happyPlayed++;
        happyPanel.SetBool("in", true);
    }
    void BackToGameplay()
    {
        FindObjectOfType<loading_screen>().startLoad("Gameplay");
    }
    public void Lose()
    {
        PlayerPrefs.SetInt("boss_cleared" + boss_index, 0);
        PlayerPrefs.SetInt("bossDoor" + boss_index, 120);
        FindObjectOfType<loading_screen>().startLoad("Gameplay");
    }
    public void GetHit()
    {
        GameObject.Find("Billy_hit").GetComponent<AudioSource>().Play();
        health--;
        if(health <= 0)
        {
            Lose();
        }
        currentShadows = new Vector4(2.5f, 0, 0, 0f);
        PlayerPrefs.SetInt("health", health);
        Shake(5);
    }
    public void Shake(int value)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = value;
        CancelInvoke("StopShake");
        Invoke("StopShake", 0.5f);
    }
    void StopShake()
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }
    public void ShowHint()
    {
        if (!isWinning)
        {
            hintPanel.SetBool("in", !hintPanel.GetBool("in"));
        }
    }
    void ChangeCanHint()
    {
        canHint = true;
    }
}
