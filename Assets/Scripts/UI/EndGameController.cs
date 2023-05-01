using System;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    private ScenesManager _scenesManager;

    private void Start()
    {
        _scenesManager = GameObject.Find("SceneManager").GetComponent<ScenesManager>();
    }

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

    public void NavigateHome()
    {
        _scenesManager.NavigateHome();
    }

    public void Replay()
    {
       _scenesManager.PlayGame();
    }
}
