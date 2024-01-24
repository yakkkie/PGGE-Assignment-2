using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private GameApp app;
    private PlayerManager playerManager;


    private void Start()
    {
        //getting reference to the game app and the manager
        app = GameObject.Find("GameApp").GetComponent<GameApp>() ;
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>() ;
    }

    public void OnClickResume()
    {
        //resume the game buy setting the game paused back to false
        app.GamePaused = false ;
    }

    public void OnclickBackToMenu()
    {
        //call the leave room here to go back to the menu
        app.GamePaused = false ;
        playerManager.LeaveRoom();
    }

    public void OnClickQuitGame()
    {
        //quit the game
        Application.Quit();
    }
}
