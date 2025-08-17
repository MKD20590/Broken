using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class loading_screen : MonoBehaviour
{
    public static loading_screen instance;
    Animator anim;
    string scene_name;
    public bool isLoading = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void loadScene()
    {
        SceneManager.LoadSceneAsync(scene_name);
        anim.SetBool("out",true);
    }
    public void doneLoad()
    {
        isLoading = false;
    }
    public void startLoad(string sceneName)
    {
        isLoading = true;
        anim.SetBool("out",false);
        scene_name = sceneName;
        anim.Play("load");
    }
}
