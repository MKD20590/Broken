using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private Heart heart;
    [SerializeField] private AudioSource steps;
    [SerializeField] private AudioSource heartbeat;
    [SerializeField] private GameObject gameoverScreen;
    [Header("front, back, left, right")]
    [SerializeField] private List<GameObject> sprites;
    public int health = 5;
    [Header("boss cleared = +33, full heart piece = +1 (last)")]
    public int sanity = 0;
    [SerializeField] private Animator fear_Text;
    [SerializeField] private ParticleSystem fear_particle;
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private List<GameObject> boss_volumes;
    [SerializeField] private List<Animator> parAnim;
    [SerializeField] private List<float> shakeValues;
    [SerializeField] private List<float> distanceValues;
    [SerializeField] private List<ParticleSystem> particle1;
    [SerializeField] private List<ParticleSystem> particle2;
    [SerializeField] private List<ParticleSystem> particle3;
    //[SerializeField] private List<ParticleSystem> particle4;

    [SerializeField] private List<LineRenderer> threads1;
    [SerializeField] private List<LineRenderer> threads2;
    [SerializeField] private List<LineRenderer> threads3;
    //[SerializeField] private List<LineRenderer> threads4;
    [SerializeField] private int segmentsPerUnit = 5;
    [Header("john - nurse - horrace")]
    [SerializeField] private List<Transform> bosses_position;
    public List<bool> bosses_cleared;
    [SerializeField] private LayerMask groundLayer;
    Rigidbody2D rb;
    [SerializeField] private float speed = 5f;
    Animator anim;
    public bool isColliding = false;
    Vector2 lastPosition;
    // Start is called before the first frame update
    void Awake()
    {
        virtualCam.m_Lens.OrthographicSize = 3f;
        health = PlayerPrefs.GetInt("health", 5);
        if(health <= 0)
        {
            GameObject.Find("bgm_main").GetComponent<AudioSource>().Stop();
            gameoverScreen.SetActive(true);
            PlayerPrefs.SetFloat("player_lastPositionX", 0);
            PlayerPrefs.SetFloat("player_lastPositionY", 0);
        }
        else
        {
            GameObject.Find("bgm_main").GetComponent<AudioSource>().Play();
            gameoverScreen.SetActive(false);
        }
        lastPosition = new Vector2(PlayerPrefs.GetFloat("player_lastPositionX", 0), PlayerPrefs.GetFloat("player_lastPositionY", 0));
        transform.position = lastPosition;
        sanity = 0;
        for (int i = 0; i < 3; i++)
        {
            bosses_cleared[i] = PlayerPrefs.GetInt("boss_cleared"+i, 0) > 0;
            if (bosses_cleared[i]) sanity += 33;
        }

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(health > 0)
        {

            //health = PlayerPrefs.GetInt("health", 3);
            //sanity = PlayerPrefs.GetInt("sanity", 0);

            lastPosition = transform.position;
            PlayerPrefs.SetFloat("player_lastPositionX", lastPosition.x);
            PlayerPrefs.SetFloat("player_lastPositionY", lastPosition.y);
            fear_Text.SetBool("in", isColliding);

            if (!bosses_cleared[0])
            {
                float distance = Vector2.Distance(bosses_position[0].position, transform.position);
                distanceValues[0] = distance;
                float speed = 10f-distance;
                if(speed <= 0.1f)
                {
                    speed = 0.1f;
                }
                float shake = 5f - distance;
                if(shake <= 0f)
                {
                    shake = 0f;
                }
                shakeValues[0] = shake;


                parAnim[0].SetFloat("speed", speed);
                foreach (ParticleSystem par in particle1)
                {
                    // Update posisi di tengah-tengah garis
                    par.transform.position = (bosses_position[0].position + transform.position) * 0.5f;

                    // Update rotasi mengarah ke player
                    Vector3 dir = (transform.position - bosses_position[0].position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    par.transform.rotation = Quaternion.Euler(0, 0, angle);

                    // Update par Length
                    ParticleSystem.ShapeModule shape = par.shape;
                    shape.scale = new Vector3(distance-1,0.5f,0);

                }

                int segmentCount = Mathf.Max(2, Mathf.RoundToInt(distance * segmentsPerUnit));
                foreach (LineRenderer thread in threads1)
                {
                    thread.positionCount = segmentCount;

                    for (int i = 0; i < segmentCount; i++)
                    {
                        float t = (float)i / (segmentCount - 1);
                        Vector3 point = Vector3.Lerp(bosses_position[0].position, transform.position, t);
                        thread.SetPosition(i, point);
                    }
                }
            }
            else
            {
                boss_volumes[0].SetActive(false);
                shakeValues[0] = 0;
                distanceValues[0] = 100;
                foreach (LineRenderer thread in threads1)
                {
                    thread.gameObject.SetActive(false);
                }
            }

            if (!bosses_cleared[1])
            {
                float distance = Vector2.Distance(bosses_position[1].position, transform.position);
                distanceValues[1] = distance;
                float speed = 10f - distance;
                if (speed <= 0.1f)
                {
                    speed = 0.1f;
                }
                float shake = 5f - distance;
                if (shake <= 0f)
                {
                    shake = 0f;
                }
                shakeValues[1] = shake;


                parAnim[1].SetFloat("speed", speed);
                foreach (ParticleSystem par in particle2)
                {
                    // Update posisi di tengah-tengah garis
                    par.transform.position = (bosses_position[1].position + transform.position) * 0.5f;

                    // Update rotasi mengarah ke player
                    Vector3 dir = (transform.position - bosses_position[1].position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    par.transform.rotation = Quaternion.Euler(0, 0, angle);

                    // Update par Length
                    ParticleSystem.ShapeModule shape = par.shape;
                    shape.scale = new Vector3(distance - 1, 0.5f, 0);

                }

                int segmentCount = Mathf.Max(2, Mathf.RoundToInt(distance * segmentsPerUnit));
                foreach (LineRenderer thread in threads2)
                {
                    thread.positionCount = segmentCount;

                    for (int i = 0; i < segmentCount; i++)
                    {
                        float t = (float)i / (segmentCount - 1);
                        Vector3 point = Vector3.Lerp(bosses_position[1].position, transform.position, t);
                        thread.SetPosition(i, point);
                    }
                }
            }
            else
            {
                boss_volumes[1].SetActive(false);
                shakeValues[1] = 0;
                distanceValues[1] = 100;
                foreach (LineRenderer thread in threads2)
                {
                    thread.gameObject.SetActive(false);
                }
            }

            if (!bosses_cleared[2])
            {
                float distance = Vector2.Distance(bosses_position[2].position, transform.position);
                distanceValues[2] = distance;
                float speed = 10f - distance;
                if (speed <= 0.1f)
                {
                    speed = 0.1f;
                }
                float shake = 5f - distance;
                if (shake <= 0f)
                {
                    shake = 0f;
                }
                shakeValues[2] = shake;


                parAnim[2].SetFloat("speed", speed);
                foreach (ParticleSystem par in particle3)
                {
                    // Update posisi di tengah-tengah garis
                    par.transform.position = (bosses_position[2].position + transform.position) * 0.5f;

                    // Update rotasi mengarah ke player
                    Vector3 dir = (transform.position - bosses_position[2].position).normalized;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    par.transform.rotation = Quaternion.Euler(0, 0, angle);

                    // Update par Length
                    ParticleSystem.ShapeModule shape = par.shape;
                    shape.scale = new Vector3(distance - 1, 0.5f, 0);

                }

                int segmentCount = Mathf.Max(2, Mathf.RoundToInt(distance * segmentsPerUnit));
                foreach (LineRenderer thread in threads3)
                {
                    thread.positionCount = segmentCount;

                    for (int i = 0; i < segmentCount; i++)
                    {
                        float t = (float)i / (segmentCount - 1);
                        Vector3 point = Vector3.Lerp(bosses_position[2].position, transform.position, t);
                        thread.SetPosition(i, point);
                    }
                }
            }
            else
            {
                boss_volumes[2].SetActive(false);
                shakeValues[2] = 0;
                distanceValues[2] = 100;
                foreach (LineRenderer thread in threads3)
                {
                    thread.gameObject.SetActive(false);
                }
            }
            cameraShake();

            //movement
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (FindObjectOfType<loading_screen>().isLoading)
            { 
                moveInput = Vector2.zero;
            }
            if (moveInput != Vector2.zero)
            {
                anim.SetBool("walking", true);
            }
            else
            {
                anim.SetBool("walking", false);
            }
            if (moveInput.y > 0)
            {
                // Move up
                sprites[1].SetActive(true);
                sprites[0].SetActive(false);
                sprites[2].SetActive(false);
                sprites[3].SetActive(false);
            }
            else if (moveInput.x > 0)
            {
                // Move right
                sprites[3].SetActive(true);
                sprites[0].SetActive(false);
                sprites[1].SetActive(false);
                sprites[2].SetActive(false);
            }
            else if(moveInput.x < 0)
            {
                // Move left
                sprites[2].SetActive(true);
                sprites[0].SetActive(false);
                sprites[1].SetActive(false);
                sprites[3].SetActive(false);
            }
            else if (moveInput.y < 0)
            {
                // Move down
                sprites[0].SetActive(true);
                sprites[1].SetActive(false);
                sprites[2].SetActive(false);
                sprites[3].SetActive(false);
            }

            rb.velocity = new Vector2(moveInput.x * speed, moveInput.y * speed);
            rb.gravityScale = 0f;
        }
            if(!heart.heartCompleted)
            {
        }
    }
    void cameraShake()
    {
        float shake = Mathf.Max(0, shakeValues[0], shakeValues[1], shakeValues[2]);
        heartbeat.volume = shake / 5f;
        //float dist_max = Mathf.Max(0f, distanceValues[0], distanceValues[1], distanceValues[2], distanceValues[3]);
        float dist_min = Mathf.Min(distanceValues[0], distanceValues[1], distanceValues[2]);
        float e_value = 40f - dist_min * 7f;
        if (e_value < 0f)
        {
            e_value = 0f;
        }
        ParticleSystem.EmissionModule emission = fear_particle.emission;
        emission.rateOverTimeMultiplier = e_value;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = shake;
    }
    public void stepping()
    {
        steps.Play();
    }
/*    public void addSanity(int value)
    {
        sanity += value;
        PlayerPrefs.SetInt("sanity", sanity);
    }*/
}
