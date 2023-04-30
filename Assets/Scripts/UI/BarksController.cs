using System.Collections;
using UnityEngine;

public class BarksController : MonoBehaviour
{
    [SerializeField] private BarkPopup barkPopupLeft;
    [SerializeField] private BarkPopup barkPopupRight;
    [SerializeField] private float minTimeBetweenBarks;

    private bool _isBarkOnScreen;
    private float _timeLastBarkClosed;

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
        if (_isBarkOnScreen || minTimeBetweenBarks < Time.time - _timeLastBarkClosed)
        {
            return;
        }

        if (damageBark)
        {
            // Get a new bark and speaker from the soul factory
            // Get a return quip for Charon 

            // calculate the bark time showing length
            // Set up bark popups and show it on screen
        }
        else
        {
            // Get an atmospheric bark and speaker
            // Set up and show back popup
        }
    }

    private IEnumerator ShowPopup(BarkPopup popup, int timeOnScreen)
    {
        // Fade popup in
        // Show for timeOnScreenSeconds

        yield return new WaitForSeconds(timeOnScreen);

        // Fade Out

        _timeLastBarkClosed = Time.time;
    }
}