using Managers;
using TMPro;
using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private TMP_Text totalSoulsLabel;
    [SerializeField] private TMP_Text carriedSoulsLabel;
    [SerializeField] private TMP_Text soulCapacityLabel;

    private void OnEnable()
    {
        GameStateManager.OnDialogueEnter += HideHud;
        GameStateManager.OnPauseEnter += HideHud;
        GameStateManager.OnPauseExit += ShowHud;
        GameStateManager.OnFerryingEnter += ShowHud;
        GameStateManager.OnReturningEnter += ShowHud;
        BoatCapacity.OnSoulsChanged += UpdateSoulDisplays;
    }

    private void OnDisable()
    {
        GameStateManager.OnDialogueEnter -= HideHud;
        GameStateManager.OnPauseEnter -= HideHud;
        GameStateManager.OnPauseExit -= ShowHud;
        GameStateManager.OnFerryingEnter -= ShowHud;
        GameStateManager.OnReturningEnter -= ShowHud;
        BoatCapacity.OnSoulsChanged -= UpdateSoulDisplays;
    }

    private void ShowHud()
    {
        ui.SetActive(true);
    }

    private void HideHud()
    {
        ui.SetActive(false);
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

    private void UpdateSoulDisplays(SoulAmounts soulAmounts)
    {
        totalSoulsLabel.text = soulAmounts.SoulsSaved.ToString();
        carriedSoulsLabel.text = soulAmounts.CurrentLoad.ToString();
        soulCapacityLabel.text = soulAmounts.CurrentCapacity.ToString();
    }
}