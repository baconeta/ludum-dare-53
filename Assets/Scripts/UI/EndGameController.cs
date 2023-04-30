using System;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
    [SerializeField] private GameObject ui;

    private void OnEnable()
    {
        GameStateManager.OnEndEnter += ShowUi;
    }

    private void OnDisable()
    {
        GameStateManager.OnEndEnter -= ShowUi;
    }

    private void ShowUi()
    {
        ui.SetActive(true);
    }

    
    //TODO Move this logic into a SceneManager class.
    public void NavigateHome()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
    
    //TODO Move this logic into a SceneManager class.
    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
