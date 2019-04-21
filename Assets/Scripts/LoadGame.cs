using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour {

    public void LoadGameSence()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadMainSence()
    {
       
        SceneManager.LoadScene(0);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void Awake()
    {
       
    }
}
