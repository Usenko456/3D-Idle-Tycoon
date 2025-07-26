using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenemanager : MonoBehaviour
{
    public InputManager inputManager;
    public GameObject menuPanel;
    public GameObject settingsuPanel;

    private bool isGameStarted = false;

    private void Start()
    {
        inputManager.OnEscape += TogglePanel;
        settingsuPanel.SetActive(false);
    }

    public void StartGame()
    {
        isGameStarted = true;
        menuPanel.SetActive(false);
        settingsuPanel.SetActive(false);

    }
    public void Settings()
    {
        isGameStarted = true;
        menuPanel.SetActive(false);
        settingsuPanel.SetActive(true);
    }
    private void TogglePanel()
    {
       
        if (isGameStarted)
        {
            bool isActive = menuPanel.activeSelf;
            menuPanel.SetActive(!isActive);
            settingsuPanel.SetActive(!isActive);
        }
        isGameStarted = false;
    }

    public void Quitgame()
    {
        Application.Quit();
    }
}
