using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class BarksController : MonoBehaviour
{
    [SerializeField] private BarkPopup barkPopupLeft;
    [SerializeField] private BarkPopup barkPopupRight;
    [SerializeField] private float minTimeBetweenBarks;
    [SerializeField] private int percentChanceToBarkPerSecond = 8;
    [SerializeField] private Sprite charonImage;
    [SerializeField] private Sprite soulImage;
    [SerializeField] private float responseDelaySeconds = 1f;

    private bool _isBarkOnScreen;
    private float _timeLastBarkClosed;
    private float _secondTimer;

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

    private void Update()
    {
        _secondTimer += Time.deltaTime;

        // Every second roll a die
        if (_secondTimer >= 1f)
        {
            if (Random.Range(0, 100) < percentChanceToBarkPerSecond)
            {
                TryBark(false);
                _secondTimer = 0f;
            }
        }
    }

    private void SetLeftSpeaker(string speaker, string bark)
    {
        barkPopupLeft.SetSpeaker(speaker);
        barkPopupLeft.SetText(bark);
        barkPopupLeft.SetSpeakerPic(GetSpeakerSprite(speaker));
    }

    private void SetRightSpeaker(string speaker, string bark)
    {
        barkPopupRight.SetSpeaker(speaker);
        barkPopupRight.SetText(bark);
        barkPopupRight.SetSpeakerPic(GetSpeakerSprite(speaker));
    }

    private float CalculateTimeOnScreen(string text)
    {
        return 3f;
    }

    private Sprite GetSpeakerSprite(string speaker)
    {
        return speaker == "Charon" ? charonImage : soulImage;
    }

    public void HitSomething()
    {
        TryBark(true);
    }

    private void TryBark(bool damageBark)
    {
        // We decide if solo bark or duo bark
        // On return journey, all barks are solo
        
        var secondsSinceLastBark = Time.time - _timeLastBarkClosed;
        if (_isBarkOnScreen || secondsSinceLastBark < minTimeBetweenBarks)
        {
            return;
        }

        _isBarkOnScreen = true;

        if (damageBark)
        {
            //TODO handle solo barks
            // Get a new bark and speaker from the soul factory
            var soulName = "James";
            var soulBark = "How dare you.";

            // Get a return quip for Charon 
            var charonBark = "Okay bro";

            // calculate the bark time showing length
            var timeOnScreen = Math.Max(CalculateTimeOnScreen(charonBark), CalculateTimeOnScreen(soulBark));

            // Set up bark popups and show it on screen
            SetLeftSpeaker(soulName, soulBark);
            SetRightSpeaker("Charon", charonBark);
            StartCoroutine(ShowPopup(barkPopupLeft, timeOnScreen));
            StartCoroutine(ShowPopup(barkPopupRight, timeOnScreen, responseDelaySeconds));
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