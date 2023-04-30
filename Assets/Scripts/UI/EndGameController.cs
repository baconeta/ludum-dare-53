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
}
