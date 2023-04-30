using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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

    private void Start()
    {
        _timeLastBarkClosed = Time.time;
    }

    private void SetLeftSpeaker(string speaker, string bark)
    {
        barkPopupLeft.SetSpeaker(speaker);
        barkPopupLeft.SetText(bark);
    }

    private void SetRightSpeaker(string speaker, string bark)
    {
        barkPopupRight.SetSpeaker(speaker);
        barkPopupRight.SetText(bark);
    }

    private void CalculateTimeOnScreen(string text)
    {
    }

    public void HitSomething()
    {
        TryBark(true);
    }

    private void TryBark(bool damageBark)
    {
        float secondsSinceLastBark = Time.time - _timeLastBarkClosed;
        if (_isBarkOnScreen || secondsSinceLastBark < minTimeBetweenBarks)
        {
            Debug.Log("Can't bark yet");
            return;
        }

        _isBarkOnScreen = true;

        if (damageBark)
        {
            Debug.Log("Damage bark");
            // Get a new bark and speaker from the soul factory
            // Get a return quip for Charon 

            // calculate the bark time showing length
            // Set up bark popups and show it on screen
            SetLeftSpeaker("Testing", "How dare you.");
            SetRightSpeaker("James", "Okay bro");
            StartCoroutine(ShowPopup(barkPopupLeft, 3));
            StartCoroutine(ShowPopup(barkPopupRight, 3, 1));
        }
        else
        {
            // Get an atmospheric bark and speaker
            // Set up and show back popup
        }
    }

    private IEnumerator ShowPopup(BarkPopup popup, float timeOnScreen, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        // Fade popup in
        popup.gameObject.SetActive(true);

        // Show for timeOnScreenSeconds
        yield return new WaitForSeconds(timeOnScreen);

        // Fade Out
        popup.gameObject.SetActive(false);
        _timeLastBarkClosed = Time.time;
        _isBarkOnScreen = false;
    }
}