using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField] private GameObject _ui;

    private void OnEnable()
    {
        GameStateManager.OnPauseEnter += HideHud;
        GameStateManager.OnFerryingEnter += ShowHud;
        GameStateManager.OnReturningEnter += ShowHud;
    }

    private void OnDisable()
    {
        GameStateManager.OnPauseEnter -= HideHud;
        GameStateManager.OnFerryingEnter -= ShowHud;
        GameStateManager.OnReturningEnter -= ShowHud;
    }

    private void ShowHud()
    {
        _ui.SetActive(true);
    }

    private void HideHud()
    {
        _ui.SetActive(false);
    }

    public void PauseGame()
    {
        GameStateManager.Instance.CurrentState = GameStateManager.GameStates.Pause;
    }
}