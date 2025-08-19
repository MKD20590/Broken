using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Animator creditPanel;
    [SerializeField] private GameObject warningPanel;
    [SerializeField] private Image warningPanel_bg;
    [SerializeField] private Volume volume;
    Vignette vignette;
    [SerializeField] private float vignette_intensity;

    bool warned = false;
    bool credit = false;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        warned = PlayerPrefs.GetInt("warned",0) > 0;
        if (PlayerPrefs.HasKey("health") || PlayerPrefs.HasKey("sanity"))
        {
            continueButton.SetActive(true);
        }
        else
        {
            continueButton.SetActive(false);
        }
        if (warned)
        {
            warningPanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(volume.profile.TryGet(out vignette))
        {
            vignette.intensity.value = vignette_intensity;
        }
        if (!warned && warningPanel_bg.color.a <= 0 && Input.GetMouseButtonDown(0))
        {
            warned = true;
            PlayerPrefs.SetInt("warned", 10);
        }
        warningPanel.transform.parent.GetComponent<Animator>().SetBool("warned",warned);
    }
    public void newGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("warned", 10);
        PlayerPrefs.SetInt("health",5);
        PlayerPrefs.SetInt("sanity",0);
        continueGame();
    }
    public void continueGame()
    {
        FindObjectOfType<loading_screen>().startLoad("gameplay");
    }
    public void creditGame()
    {
        credit = !credit;
        creditPanel.SetBool("in", credit);
    }
    public void quitGame()
    {
        Application.Quit();
    }
}
