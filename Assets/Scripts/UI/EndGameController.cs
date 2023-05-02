using Managers;
using TMPro;
using UnityEngine;

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
        DialogueManager.OnDialogueEnd += ShowUi;
    }

    private void OnDisable()
    {
        DialogueManager.OnDialogueEnd -= ShowUi;
    }

    private void ShowUi()
    {
        if (GameStateManager.Instance.CurrentState != GameStateManager.GameStates.End) return;
        
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
