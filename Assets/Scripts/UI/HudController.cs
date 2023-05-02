using Managers;
using TMPro;
using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    [SerializeField] private TMP_Text totalSoulsLabel;
    [SerializeField] private TMP_Text carriedSoulsLabel;
    [SerializeField] private TMP_Text soulCapacityLabel;
    [SerializeField] private GameObject joystickUI;
    private bool _isVisible;

    [SerializeField] private Animator capacityAnimator;
    [SerializeField] private Animator totalAnimator;
    private static readonly int SoulsUpdated = Animator.StringToHash("SoulsUpdated");

    private void OnEnable()
    {
        //GameState Show/Hide Hud
        GameStateManager.OnDialogueEnter += HideHud;
        GameStateManager.OnPauseEnter += HideHud;
        GameStateManager.OnPauseExit += ShowHud;
        GameStateManager.OnFerryingEnter += ShowHud;
        GameStateManager.OnReturningEnter += ShowHud;
        GameStateManager.OnEndEnter += HideHud;
        
        BoatCapacity.OnSoulsChanged += UpdateSoulDisplays;
        
        InputManager.onControlSchemeChange += ToggleJoystick;

        BoatController.OnVoyageComplete += UpdateTotal;
        BoatController.OnDamageTaken += UpdateCapacity;
    }

    private void OnDisable()
    {
        GameStateManager.OnDialogueEnter -= HideHud;
        GameStateManager.OnPauseEnter -= HideHud;
        GameStateManager.OnPauseExit -= ShowHud;
        GameStateManager.OnFerryingEnter -= ShowHud;
        GameStateManager.OnReturningEnter -= ShowHud;
        GameStateManager.OnEndEnter -= HideHud;
        
        BoatCapacity.OnSoulsChanged -= UpdateSoulDisplays;
        
        InputManager.onControlSchemeChange -= ToggleJoystick;
        
        BoatController.OnVoyageComplete -= UpdateTotal;
        BoatController.OnDamageTaken -= UpdateCapacity;
    }

    private void ShowHud()
    {
        ui.SetActive(true);
        
    }

    private void HideHud()
    {
        ui.SetActive(false);
    }
    

    public void ToggleJoystick(bool toggleOn)
    {
       
        joystickUI.SetActive(toggleOn);
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

    private void UpdateTotal() {
        // We check for returning here because the state is changed before the event is fired
        if (GameStateManager.Instance.CurrentState != GameStateManager.GameStates.Returning) return;
        
        totalAnimator.SetTrigger(SoulsUpdated);
    }

    private void UpdateCapacity() {
        if (!GameStateManager.Instance.IsGameActive()) return;
        
        capacityAnimator.SetTrigger(SoulsUpdated);
    }

    private void UpdateSoulDisplays(SoulAmounts soulAmounts)
    {
        totalSoulsLabel.text = soulAmounts.SoulsSaved.ToString();
        carriedSoulsLabel.text = soulAmounts.CurrentLoad.ToString();
        soulCapacityLabel.text = soulAmounts.CurrentCapacity.ToString();
    }
}