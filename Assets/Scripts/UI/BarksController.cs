using System;
using System.Collections;
using Managers;
using UnityEngine;
using Random = UnityEngine.Random;

public class BarksController : MonoBehaviour
{
    [SerializeField] private SoulFactory soulFactory;
    [SerializeField] private CharonBarkManager charonBarkManager;
    [SerializeField] private BarkPopup barkPopupLeft;
    [SerializeField] private BarkPopup barkPopupRight;
    [SerializeField] private float minTimeBetweenBarks;
    [SerializeField] private int percentChanceToBarkPerSecond = 8;
    [SerializeField] private Sprite charonImage;
    [SerializeField] private Sprite soulImage;
    [SerializeField] private float responseDelaySeconds = 1f;
    [SerializeField] private int percentChanceToBeSoloBark = 50;
    [SerializeField] private float minTimeBarkOnScreen = 3f; // 16 chars
    [SerializeField] private float maxTimeBarkOnScreen = 5.5f; // 72 chars

    private bool _isBarkOnScreen;
    private float _timeLastBarkClosed;
    private float _secondTimer;
    private bool _canBark;

    private void OnEnable()
    {
        BoatController.OnDamageTaken += HitSomething;
        BoatController.OnVoyageStart += StartBarkTimers;
        BoatController.OnVoyageComplete += StopBarkTimers;
        BoatCapacity.OnBoatDestroyed += StopBarkTimers;
        BoatCapacity.OnAllSoulsLost += StopBarkTimers;
        GameStateManager.OnPauseEnter += StopBarkTimers;
        GameStateManager.OnPauseExit += StartBarkTimers;
        DialogueManager.OnDialogueStart += CloseAllPopups;
    }

    private void OnDisable()
    {
        BoatController.OnDamageTaken -= HitSomething;
        BoatController.OnVoyageStart -= StartBarkTimers;
        BoatController.OnVoyageComplete -= StopBarkTimers;
        BoatCapacity.OnBoatDestroyed -= StopBarkTimers;
        BoatCapacity.OnAllSoulsLost -= StopBarkTimers;
        GameStateManager.OnPauseEnter -= StopBarkTimers;
        GameStateManager.OnPauseExit -= StartBarkTimers;
        DialogueManager.OnDialogueStart -= CloseAllPopups;
    }

    private void Start()
    {
        _timeLastBarkClosed = Time.time;
    }

    private void Update()
    {
        if (!_canBark) return;

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

    private void StartBarkTimers()
    {
        _canBark = true;
    }

    private void StopBarkTimers()
    {
        _canBark = false;
    }


    private void CloseAllPopups()
    {
        StopAllCoroutines();
        _timeLastBarkClosed = Time.time;
        _isBarkOnScreen = false;
        barkPopupLeft.gameObject.SetActive(false);
        barkPopupRight.gameObject.SetActive(false);
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
        if (text == null) return 2f;
        
        var normalizedValue = Mathf.InverseLerp(16, 90, text.Length);
        var mappedValue = Mathf.Lerp(minTimeBarkOnScreen, maxTimeBarkOnScreen, normalizedValue);
        return mappedValue;
    }

    private Sprite GetSpeakerSprite(string speaker)
    {
        return speaker == "Charon" ? charonImage : soulImage;
    }

    public void HitSomething()
    {
        if (!_canBark) return;
        TryBark(true);
    }

    private void TryBark(bool damageBark)
    {
        var secondsSinceLastBark = Time.time - _timeLastBarkClosed;
        if (_isBarkOnScreen || secondsSinceLastBark < minTimeBetweenBarks)
        {
            return;
        }

        var returning = GameStateManager.Instance.CurrentState == GameStateManager.GameStates.Returning;
        var soloBark = returning || Random.Range(0, 100) < percentChanceToBeSoloBark;

        _isBarkOnScreen = true;
        if (!soloBark && damageBark)
        {
            ResponseDamageBark();
        }

        else if (returning)
        {
            CharonOnlyBark(damageBark);
        }
        else
        {
            SoulOnlyBark(damageBark);
        }
    }

    private void ResponseDamageBark()
    {
        Soul soul = soulFactory.GenerateRandomSoul();
        var soulName = soul.Name;
        var soulBark = soul.GetDuetDamageBark();

        var charonBark = charonBarkManager.GetCharonDuetDamageBark();

        // Calculate the bark time showing length
        var timeOnScreen = Math.Max(CalculateTimeOnScreen("Charon" + charonBark), CalculateTimeOnScreen(soul.Name + soulBark));

        // Set up bark popups and show it on screen
        SetLeftSpeaker(soulName, soulBark);
        SetRightSpeaker("Charon", charonBark);
        StartCoroutine(ShowPopup(barkPopupLeft, timeOnScreen));
        StartCoroutine(ShowPopup(barkPopupRight, timeOnScreen, responseDelaySeconds));
    }

    private void SoulOnlyBark(bool hit = false)
    {
        Soul soul = soulFactory.GenerateRandomSoul();
        var soulName = soul.Name;
        var soulBark = hit ? soul.GetDamageBark() : soul.GetAmbienceBark();

        // calculate the bark time showing length
        var timeOnScreen = CalculateTimeOnScreen(soul.Name + soulBark);

        // Set up bark popups and show it on screen
        SetLeftSpeaker(soulName, soulBark);
        StartCoroutine(ShowPopup(barkPopupLeft, timeOnScreen));
    }

    private void CharonOnlyBark(bool hit = false)
    {
        var charonBark = hit ? charonBarkManager.GetCharonDamageBark() : charonBarkManager.GetCharonAmbienceBark();

        // calculate the bark time showing length
        var timeOnScreen = CalculateTimeOnScreen(charonBark);

        // Set up bark popups and show it on screen
        SetLeftSpeaker("Charon", charonBark);
        StartCoroutine(ShowPopup(barkPopupLeft, timeOnScreen));
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