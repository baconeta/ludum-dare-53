using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenController : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    private ScenesManager _scenesManager;

    private void Start()
    {
        _scenesManager = GameObject.Find("SceneManager").GetComponent<ScenesManager>();
    }

    private void OnEnable()
    {
        GameStateManager.OnPauseEnter += ShowUi;
        GameStateManager.OnPauseExit += HideUi;
    }

    private void OnDisable()
    {
        GameStateManager.OnPauseEnter -= ShowUi;
        GameStateManager.OnPauseExit -= HideUi;
    }

    private void ShowUi()
    {
        ui.SetActive(true);
    }

    private void HideUi()
    {
        ui.SetActive(false);
    }

    public void ResumePlaying()
    {
        HideUi();
        GameStateManager.Instance.Resume();
    }

    public void QuitToMenu()
    {
        _scenesManager.NavigateHome();
    }
}
