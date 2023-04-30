using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenController : MonoBehaviour
{
    [SerializeField] private GameObject ui;

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
}
