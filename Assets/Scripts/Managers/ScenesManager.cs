using System;
using Audio;
using UI.StateSwitcher;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private static bool _clickedContinue;

    [SerializeField] private CompositeStateSwitcher skipContinueScreen;
    [SerializeField] private MusicController bgMusicStarter;
    
    // Switch to the game scene
    public void PlayGame()
    {
        SceneManager.LoadScene("Scenes/GameScene");
    }
    
    // Switch to the main menu scene
    public void NavigateHome()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            return;
        }
        if (_clickedContinue)
        {
            skipContinueScreen.ChangeState("SplashScreen");
            bgMusicStarter.PlayManually();
        }
        else
        {
            _clickedContinue = true;
        }
    }
}
