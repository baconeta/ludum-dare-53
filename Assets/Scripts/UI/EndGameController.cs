using System;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private TMP_Text soulsCount;
    [SerializeField] private BoatCapacity boatCapacity;
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
        soulsCount.SetText(boatCapacity.SoulsSaved.ToString());
        PlayerPrefs.SetInt("TotalSoulsSaved", PlayerPrefs.GetInt("TotalSoulsSaved", 0) + boatCapacity.SoulsSaved);
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
