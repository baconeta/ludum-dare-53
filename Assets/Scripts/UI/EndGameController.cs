using System;
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

    private void Start()
    {
        //TODO Remove and integrate into GameState system.
        //Currently disables UI to stop it showing :)
        _ui.SetActive(false);
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
