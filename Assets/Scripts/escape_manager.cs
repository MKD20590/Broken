using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class escape_manager : MonoBehaviour
{
    //[SerializeField] private Animator gameover;
    [SerializeField] private List<ParticleSystem> fail_particle;
    [SerializeField] private List<GameObject> prefab;
    [SerializeField] private List<Transform> spawn_positions;
    [SerializeField] private Animator endingScreen;
    [SerializeField] private AudioSource bgm_main;
    //[SerializeField] private AudioSource bgm_lose;
    [SerializeField] private AudioSource bgm_ending;
    [SerializeField] private Slider timer;
    bool isWin = false;
    int health = 5;
    [SerializeField] private float obstacleRange = 4f;
    [SerializeField] private float obstacleSpeed = 6f;
    float running_time = 0f;
    float current_time = 0f;
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private Volume volume;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider healthBar_lerp;
    ShadowsMidtonesHighlights shadowsMidtonesHighlights;
    Vector4 currentShadows;
    List<GameObject> currentObstacle = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        shake(0);
        health = PlayerPrefs.GetInt("health", 5);
        virtualCam.m_Lens.OrthographicSize = 5f;
        volume.profile = Instantiate(volume.profile);
        if (volume.profile.TryGet<ShadowsMidtonesHighlights>(out shadowsMidtonesHighlights))
        {
            shadowsMidtonesHighlights.shadows.overrideState = true;
            currentShadows = shadowsMidtonesHighlights.shadows.value;
            shadowsMidtonesHighlights.shadows.value = new Vector4(0, 0, 0, 0.3f);
        }
        running_time = bgm_main.clip.length/2f;
        timer.maxValue = running_time;
        timer.value = 0f;
        StartCoroutine(spawnObstacle());
    }

    // Update is called once per frame
    void Update()
    {
        //bar
        healthBar.value = health;
        healthBar_lerp.value = Mathf.MoveTowards(healthBar_lerp.value, health, Time.deltaTime * 2f);

        currentShadows = Vector4.Lerp(currentShadows, new Vector4(0, 0, 0, 0.3f), Time.deltaTime * 0.7f);
        shadowsMidtonesHighlights.shadows.value = currentShadows;

        float temp = Mathf.InverseLerp(0f,running_time, current_time);
        //temp /= 10f;
        obstacleRange = Mathf.Lerp(4f, 1f, temp);
        obstacleSpeed = Mathf.Lerp(6f, 15f, temp);

        if (current_time < running_time && !isWin && health > 0)
        {
            current_time += Time.deltaTime;
            timer.value = current_time;
        }
        else if( current_time >= running_time && !isWin && health > 0)
        {
            bgm_ending.Play();
            isWin = true;
            Invoke("ending", 24.5f);
            endingScreen.SetBool("in", true);
            timer.value = current_time;
        }
        if(isWin && health > 0)
        {
            if(currentObstacle.Count > 0)
            {
                foreach (GameObject obstacle in currentObstacle)
                {
                    if(obstacle != null)
                    {
                        Destroy(obstacle);
                    }
                }
                currentObstacle.Clear();
            }
            bgm_main.volume = Mathf.MoveTowards(bgm_main.volume, 0f, Time.deltaTime * 1f);
            if(bgm_main.volume <= 0.001f)
            {
                bgm_ending.volume = Mathf.MoveTowards(bgm_ending.volume, 1f, Time.deltaTime * 1f);
            }
        }
        if(health <= 0 && !fail_particle[0].isEmitting)
        {
            if (currentObstacle.Count > 0)
            {
                foreach (GameObject obstacle in currentObstacle)
                {
                    if (obstacle != null)
                    {
                        Destroy(obstacle);
                    }
                }
                currentObstacle.Clear();
            }
            endingScreen.SetBool("in", false);
            fail_particle[0].Play();
            fail_particle[1].Play();
            Invoke("lose", 3f);
        }
    }
    IEnumerator spawnObstacle()
    {
        yield return new WaitForSeconds(obstacleRange);
        while (!isWin && health > 0)
        {
            int idx = Random.Range(0, prefab.Count);
            currentObstacle.Add(Instantiate(prefab[idx], spawn_positions[idx]));
            currentObstacle[currentObstacle.Count-1].GetComponent<Rigidbody2D>().velocity = new Vector2(-obstacleSpeed, 0f);
            yield return new WaitForSeconds(obstacleRange);
            Invoke("destroyObject", 5f);
        }
    }
    void destroyObject()
    {
        if(currentObstacle.Count <= 0) return;
        Destroy(currentObstacle[0]);
        currentObstacle.RemoveAt(0);
    }
/*    public void restart()
    {
        FindObjectOfType<loading_screen>().startLoad("escape");
    }*/
    public void getHit()
    {
        GameObject.Find("Billy_hit").GetComponent<AudioSource>().Play();
        health--;
        shake(5);
        PlayerPrefs.SetInt("health", health);
        currentShadows = new Vector4(2.5f, 0, 0, 0f);
        if (health <= 0)
        {
            fail_particle[0].Play();
            fail_particle[1].Play();
            Invoke("lose", 3f);
            //gameover.SetBool("in", true);
        }
    }
    void lose()
    {
        FindObjectOfType<loading_screen>().startLoad("gameplay");
    }
    void ending()
    {
        PlayerPrefs.DeleteAll();
/*        PlayerPrefs.SetInt("warned", 10);
        PlayerPrefs.SetInt("health", 5);
        PlayerPrefs.SetInt("sanity", 0);*/
        FindObjectOfType<loading_screen>().startLoad("main_menu");
    }
    void shake(int value)
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
}
