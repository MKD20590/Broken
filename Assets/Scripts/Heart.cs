using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [Header("john - nurse - horrace - pojok kanan atas - fountain")]
    [SerializeField] private List<Image> pieces;
    [SerializeField] private List<GameObject> piecesUI;
    [SerializeField] private List<bool> collected;
    [SerializeField] private List<Material> heartMat;
    [SerializeField] private Animator inventoryPanel;
    [SerializeField] private bool isManager = false;
    [SerializeField] private Heart manager;
    [SerializeField] private int index;
    bool isColliding = false;
    Animator anim;
    Player player;
    bool isOpen = false;
    bool isFull = false;
    public static bool isCompleted = false;
    public bool canOpenPanel = true;
    [SerializeField] private bool lastPiece = false;
    [SerializeField] private bool thirdPiece = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        if (isManager)
        {
            for (int i = 0; i < player.bosses_cleared.Count; i++)
            {
                if (player.bosses_cleared[i])
                {
                    getPiece(i, true);
                }
                else
                {
                    getPiece(i, false);
                }
            }
            for (int i = 0; i < collected.Count; i++)
            {
                collected[i] = PlayerPrefs.GetInt("piece" + i, 0) > 0;
                if (collected[i])
                {
                    getPiece(i, true);
                }
                else
                {
                    getPiece(i, false);
                }
            }
        }
        else if (lastPiece)
        {
            bool canSpawn = true;
            for (int i = 0; i < player.bosses_cleared.Count; i++)
            {
                if (!player.bosses_cleared[i])
                {
                    canSpawn = false;
                    break;
                }
            }
            if (!manager.collected[3])
            {
                canSpawn = false;
            }
            if (canSpawn)
            {
                GetComponent<BoxCollider2D>().enabled = true;
                transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            }
            else
            {
                GetComponent<BoxCollider2D>().enabled = false;
                transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
            }
        }
        else if(thirdPiece)
        {
            if (PlayerPrefs.GetInt("piece" + index, 0) <= 0)
            {
                GetComponent<BoxCollider2D>().enabled = true;
                transform.GetChild(0).GetComponent<ParticleSystem>().Play();
            }
            else
            {
                GetComponent<BoxCollider2D>().enabled = false;
                transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isManager)
        {
            if (canOpenPanel && inventoryPanel.GetBool("in") && Input.GetMouseButtonDown(0))
            {
                openInventory();
            }
            for (int i=0;i<pieces.Count;i++)
            {
                if(collected[i])
                {
                    pieces[i].color = Color.Lerp(pieces[i].color, Color.white, Time.deltaTime * 2f);
                }
                else
                {
                    pieces[i].color = Color.Lerp(pieces[i].color, Color.clear, Time.deltaTime * 2f);
                }
            }
            if(isFull && isCompleted)
            {
                heartMat[0].SetColor("_Color", Color.Lerp(heartMat[0].GetColor("_Color"), heartMat[0].GetColor("_Color") * 3f, Time.deltaTime * 3f));
                //heartMat[1].SetColor("_Color", Color.Lerp(heartMat[1].GetColor("_Color"), heartMat[1].GetColor("_Color") * 2f, Time.deltaTime * 3f));
                anim.SetBool("full", true);
            }
        }
        else
        {
            anim.SetBool("in", isColliding);
            if (isColliding && Input.GetKeyDown(KeyCode.E))
            {
                manager.getPiece(index, true);
                GetComponent<BoxCollider2D>().enabled = false;
                transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                //gameObject.SetActive(false);
            }
        }
    }
    public void getPiece(int index, bool isOn)
    {
        if(isManager)
        {
            //john - nurse - horrace - pojok kanan atas - fountain
            if(isOn)
            {
                PlayerPrefs.SetInt("piece" + index, 10);
                collected[index] = true;
                piecesUI[index].SetActive(true);
            }
            else
            {
                PlayerPrefs.SetInt("piece" + index, 0);
                collected[index] = false;
                piecesUI[index].SetActive(false);
            }
            foreach(bool b in collected)
            {
                if(!b)
                {
                    isFull = false;
                    break;
                }
                else
                {
                    isFull = true;
                }
            }
/*            if(isFull)
            {
                canOpenPanel = true;
                openInventory();
            }*/
        }
    }
    public void completeHeart()
    {
        if(isManager)
        {
            for(int i=0;i<collected.Count;i++)
            {
                if(!collected[i])
                {
                    FindObjectOfType<fountain>().GetComponent<Animator>().ResetTrigger("warning");
                    FindObjectOfType<fountain>().GetComponent<Animator>().SetTrigger("warning");
                    return;
                }
            }
            isCompleted = true;
            canOpenPanel = true;
            openInventory();
        }
    }
    public void openInventory()
    {
        if(isManager && canOpenPanel)
        {
            isOpen = !isOpen;
            inventoryPanel.SetBool("in",isOpen);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isColliding = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isColliding = false;
        }
    }
    public void hover()
    {
        if(isManager) canOpenPanel = false;
    }
    public void unhover()
    {
        if (isManager) canOpenPanel = true;
    }
}
