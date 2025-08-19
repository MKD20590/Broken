using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class enter_bossStage : MonoBehaviour
{
    public int cooldown = 0;
    float cooldownRemaining = 0f;
    bool canEnter = true;
    bool isColliding = false;
    [Header("samain kyk script _gm di bossnya, mulai dr 0")]
    [SerializeField] private int boss_index;
    [SerializeField] private TextMeshPro cooldownText;
    Player player;
    CircleCollider2D circleCollider;
    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        player = FindObjectOfType<Player>();
        cooldown = PlayerPrefs.GetInt("bossDoor" + boss_index, 0);
        PlayerPrefs.SetInt("bossDoor" + boss_index, 0);
        cooldownRemaining = cooldown;
        if (cooldownRemaining > 0 || PlayerPrefs.GetInt("boss_cleared" + boss_index, 0) > 0)
        {
            if(PlayerPrefs.GetInt("boss_cleared" + boss_index, 0) > 0) circleCollider.enabled = false;
            canEnter = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (cooldownRemaining > 0)
        {
            cooldownRemaining -= Time.deltaTime;
                
            int menit = Mathf.FloorToInt(cooldownRemaining / 60);
            int detik = Mathf.FloorToInt(cooldownRemaining % 60);
            if(isColliding) cooldownText.text = string.Format("{0:00}:{1:00}", menit, detik);
        }
        else if(cooldownRemaining <= 0 && isColliding)
        {
            cooldownText.text = "Face your fear?\n[E]";
        }
        if (isColliding && cooldownRemaining <= 0 && canEnter == true && Input.GetKeyDown(KeyCode.E))
        {
            if (boss_index == 0)
            {
                //john
                FindObjectOfType<loading_screen>().startLoad("john");
            }
            else if (boss_index == 1)
            {
                //nurse
                FindObjectOfType<loading_screen>().startLoad("nurse");
            }
            else if (boss_index == 2)
            {
                //horrace
                FindObjectOfType<loading_screen>().startLoad("horrace");
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            player.isColliding = true;
            isColliding = true;
            canEnter = true;
        }
/*        if (canEnter && Input.GetKeyDown(KeyCode.E))
        {
            canEnter = false;
        }*/
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            player.isColliding = false;
            isColliding = false;
        }
    }
/*    public void addSanity()
    {
        if (PlayerPrefs.GetInt("boss_cleared" + boss_index, 0) > 0)
        {
            if (player.bosses_cleared[boss_index])
            {
                player.addSanity(33);
            }
        }
    }*/
}
