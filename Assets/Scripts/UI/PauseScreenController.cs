using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenController : MonoBehaviour
{
    [SerializeField] private GameObject _ui;

    private void OnEnable()
    {
        GameStateManager.OnPauseEnter += ShowUi;
    }

    private void OnDisable()
    {
        GameStateManager.OnPauseEnter -= ShowUi;
    }

    private void ShowUi()
    {
        _ui.SetActive(true);
    }

    public void ResumePlaying()
    {
        _ui.SetActive(false);
        GameStateManager.Instance.Resume();
    }

    public void NavigateHome()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }
}
