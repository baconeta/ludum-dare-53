using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenController : MonoBehaviour
{
    [SerializeField] private GameObject _ui;

    private void OnEnable()
    {
        GameStateManager.OnPauseEnter += ShowUi;
        GameStateManager.OnFerryingEnter += HideUi;
        GameStateManager.OnReturningEnter += HideUi;
    }

    private void OnDisable()
    {
        GameStateManager.OnPauseEnter -= ShowUi;
        GameStateManager.OnFerryingEnter -= HideUi;
        GameStateManager.OnReturningEnter -= HideUi;
    }

    private void ShowUi()
    {
        _ui.SetActive(true);
    }

    private void HideUi()
    {
        _ui.SetActive(false);
    }

    public void ResumePlaying()
    {
        HideUi();
        GameStateManager.Instance.Resume();
    }

    public void NavigateHome()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
