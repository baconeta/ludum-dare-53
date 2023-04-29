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

    /// <summary>
    /// Pause / Resume game when Escape is pressed
    /// </summary>
    void OnGUI()
    {
        Event e = Event.current;
        if (!e.isKey || e.type != EventType.KeyUp || e.keyCode != KeyCode.Escape) return;

        if (GameStateManager.Instance.CurrentState == GameStateManager.GameStates.Pause)
        {
            GameStateManager.Instance.Resume();
        } else if (GameStateManager.Instance.IsGameActive())
        {
            PauseGame();
        }
    }
}