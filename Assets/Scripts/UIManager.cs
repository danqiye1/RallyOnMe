using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject ControlMenu;
    public GameObject BriefingMenu;

    // Play the game
    public void PlayGame(){
        SceneManager.LoadScene(1);
    }

    public void ShowBriefing()
    {
        MainMenu.SetActive(false);
        BriefingMenu.SetActive(true);
    }

    public void ShowControls()
    {
        MainMenu.SetActive(false);
        ControlMenu.SetActive(true);
    }

    public void BackToMenu()
    {
        BriefingMenu.SetActive(false);
        ControlMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
