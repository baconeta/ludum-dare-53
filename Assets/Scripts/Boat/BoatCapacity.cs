using System;
using Audio;
using Managers;
using Unity.Collections;
using UnityEngine;

public class BoatCapacity : MonoBehaviour
{
    [Header("Variables")]

    [Tooltip("The amount of souls that can fit at the boat when the game starts.")]
    [SerializeField]
    private int startingCapacity = 50;

    [Tooltip("The amount of capacity restored upon ferrying souls successfully.")]
    [SerializeField]
    private int capacityRestoredOnSuccessfulFerry = 1;
    
    public bool loseObstacleDamageOnCollision;
    [Tooltip("IF loseObstacleDamageOnCollision is false." +
        "\nThe amount of capacity lost upon hitting an obstacle with no souls.")]
    [SerializeField]
    private int lostCapacityOnDamage = 1;

    [Tooltip("**Currently Disabled: See Start Method** Does the boat start loaded to capacity? or does it start empty?")]
    [SerializeField] [ReadOnlyAttribute]
    private bool doesStartLoaded = false;

    [Tooltip("Does the boat capacity get reduced when taking damage while the boat contains souls?")]
    [SerializeField]
    private bool doesLoseCapacityWhileContainsSouls = false;

    [Tooltip("Every X souls you collect, the damage of everything increases by 1")] [SerializeField]
    private float perSoulDamage = 30f; 

    [Header("Statistics")]

    [SerializeField] private int _currentCapacity;


    public int CurrentCapacity
    {
        get { return _currentCapacity; }
        private set { _currentCapacity = value; }

    }

    [SerializeField] private int _currentLoad;
    public int CurrentLoad
    {
        get { return _currentLoad; }
        private set { _currentLoad = value; }
    }

    [SerializeField] private int _soulsSaved;
    public int SoulsSaved
    {
        get { return _soulsSaved; }
        private set { _soulsSaved += value; }
    }

    [SerializeField] private int _soulsDamned;
    public int SoulsDamned
    {
        get { return _soulsDamned; }
        private set { _soulsDamned += value; }
    }


    public static event Action OnBoatDestroyed;
    public static event Action OnAllSoulsLost;
    public static event SoulsChanged OnSoulsChanged;
    public static event Action OnCapacityChange;
    public static event Action OnSoulChange;

    private void OnEnable()
    {
        GameStateManager.OnReturningEnter += SaveAllSouls;
        GameStateManager.OnReturningEnter += IncreaseCapacity;
        GameStateManager.OnFerryingEnter += FillSouls;
    }

    private void OnDisable()
    {
        GameStateManager.OnReturningEnter -= SaveAllSouls;
        GameStateManager.OnReturningEnter -= IncreaseCapacity;
        GameStateManager.OnFerryingEnter -= FillSouls;
    }

    // Start is called before the first frame update
    private void Start()
    {
        ResetCapacity();

        //TODO Fix
        //Weird math happens here, because GameStateManager.OnFerryingEnter calls FillSouls when the game starts. True results in initial load being -Int.MaxValue.
        //CurrentLoad = doesStartLoaded ? startingCapacity : 0;

        NotifySoulsChanged();
    }

    /// <summary>
    /// Increase the capacity of the boat.
    /// </summary>
    /// <param name="amount">By how many units should the maximum capacity be increased.</param>
    private void IncreaseCapacity(int amount)
    {
        CurrentCapacity += amount;
        NotifySoulsChanged();
    }

    private void IncreaseCapacity()
    {
        IncreaseCapacity(capacityRestoredOnSuccessfulFerry);
    }

    private int DecreaseCapacity(int amount)
    {
        CurrentCapacity -= amount;
        if (CurrentCapacity <= 0)
        {
            OnBoatDestroyed?.Invoke();
        }
        return ReduceLoadToFitCapacity();
    }

    /// <summary>
    /// Reset the currentCapacity to the startingCapacity. Trim excess currentLoad to match new capacity.
    /// <returns>The number of units that were lost.</returns>
    /// </summary>
    private int ResetCapacity()
    {
        CurrentCapacity = startingCapacity;
        return ReduceLoadToFitCapacity();
    }

    /// <summary>
    /// Deal damage to the boat, potentially causing the boat to trigger game-ending events.
    /// </summary>
    /// <param name="damageToTake">How many souls or how much capacity the boat should lose.</param>
    public void DealDamageToBoat(int damageToTake)
    {
        damageToTake += Mathf.FloorToInt(_soulsSaved / perSoulDamage);
        // If we have no souls on the ferry, and the game is still running, we must be Returning.
        if (CurrentLoad == 0)
        {
            if (!loseObstacleDamageOnCollision) damageToTake = lostCapacityOnDamage;
            DecreaseCapacity(damageToTake);
            OnCapacityChange?.Invoke();
        }
        // Otherwise, we must be Ferrying.
        else
        {
            //Add the Souls Lost to the SoulsDamned statistic
            SoulsDamned = DecreaseSouls(damageToTake);
            OnSoulChange?.Invoke();

            // If we reduce the number of souls to zero by taking damage, end the game.
            if (CurrentLoad <= 0) { OnAllSoulsLost?.Invoke(); }

            if (doesLoseCapacityWhileContainsSouls)
            {
                DecreaseCapacity(damageToTake);
                OnCapacityChange?.Invoke();
            }
        }
        NotifySoulsChanged();

        if (CurrentCapacity <= 0) {
            OnBoatDestroyed?.Invoke();
        }

    }

    /// <summary>
    /// Add a number of units to the load of the boat. Excess units that couldn't be loaded are returned.
    /// </summary>
    /// <param name="loadToAdd">How many units are we trying to add to the load.</param>
    /// <returns>The number of units that were not able to fit onto the boat due to load capacity limits.</returns>
    private int IncreaseSouls(int loadToAdd = 1_000_000)
    {
        CurrentLoad += loadToAdd;
        return ReduceLoadToFitCapacity();
    }

    private void FillSouls()
    {
        IncreaseSouls();
        NotifySoulsChanged();
    }

    private int DecreaseSouls(int loadToRemove)
    {
        // Play the SFX.
        AudioWrapper.Instance.PlaySound("damned-soul-sfx");
        // Reduce souls.
        if (CurrentLoad <= loadToRemove)
        {
            return UnloadAll();
        }
        else
        {
            CurrentLoad -= loadToRemove;
            return loadToRemove;
        }

    }

    //This is run via the event Action GameStateManager.OnReturningEnter
    private void SaveAllSouls()
    {
        //Add the Souls Lost to the SoulsDamned statistic
        SoulsSaved = UnloadAll();
        NotifySoulsChanged();
    }

    private int UnloadAll()
    {
        int load = CurrentLoad;
        CurrentLoad = 0;
        return load;
    }

    /// <summary>
    /// Remove excess load 
    /// </summary>
    /// <returns>The number of units that were lost.</returns>
    private int ReduceLoadToFitCapacity()
    {
        // Excess load is lost.
        if (CurrentLoad > CurrentCapacity)
        {
            int excess = CurrentLoad - CurrentCapacity;
            CurrentLoad = CurrentCapacity;
            return excess;
        }
        else
        {
            return 0;
        }

    }

    private void NotifySoulsChanged()
    {
        var amounts = new SoulAmounts { CurrentLoad = CurrentLoad, CurrentCapacity = CurrentCapacity, SoulsSaved = SoulsSaved };
        OnSoulsChanged(amounts);
    }
}

public delegate void SoulsChanged(SoulAmounts soulAmounts);

public struct SoulAmounts
{
    public int CurrentLoad;
    public int CurrentCapacity;
    public int SoulsSaved;
}
