using UnityEngine;

public class BarksController : MonoBehaviour
{
    [SerializeField] private BarkPopup barkPopupLeft;
    [SerializeField] private BarkPopup barkPopupRight;
    [SerializeField] private float minTimeBetweenBarks;

    private bool _isBarkOnScreen;
    private float _timeSinceBarkClosed;

    private void OnEnable()
    {
        BoatController.OnDamageTaken += HitSomething;
    }

    private void OnDisable()
    {
        BoatController.OnDamageTaken -= HitSomething;
    }

    private void ShowLeftSpeaker(string speaker, string bark)
    {
        barkPopupLeft.SetSpeaker(speaker);
        barkPopupLeft.SetText(bark);
    }

    private void ShowRightSpeaker(string speaker, string bark)
    {
        barkPopupRight.SetSpeaker(speaker);
        barkPopupRight.SetText(bark);
    }

    private void CalculateTimeOnScreen(string text)
    {
    }

    private void HitSomething()
    {
        TryBark(true);
    }

    private void TryBark(bool damageBark)
    {
        if (_isBarkOnScreen || _timeSinceBarkClosed < Time.time - minTimeBetweenBarks)
        {
            return;
        }

        if (damageBark)
        {
            // Get a new bark and speaker from the soul factory
            // Get a return quip for Charon 
            // Set up bark popups and show it on screen
        }
        else
        {
            // Get an atmospheric bark and speaker
            // Set up and show back popup
        }
    }
}