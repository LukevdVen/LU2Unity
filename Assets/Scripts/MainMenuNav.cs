using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuNav : MonoBehaviour
{
    public void ToMainGame()
    {
        SceneManager.LoadSceneAsync("MainGame");
        Debug.Log("Button clicked");
    }
}
