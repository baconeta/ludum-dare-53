using System;
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
    
    [Tooltip("**Currently Disabled: See Start Method** Does the boat start loaded to capacity? or does it start empty?")]
    [SerializeField] [ReadOnlyAttribute]
    private bool doesStartLoaded = false;

    [Tooltip("Does the boat capacity get reduced when taking damage while the boat contains souls?")]
    [SerializeField]
    private bool doesLoseCapacityWhileContainsSouls = false;
    
    

    [Header("Statistics")]
    
    [SerializeField] private int _currentCapacity;
    public int CurrentCapacity
    {
        get { return _currentCapacity;}
        private set { _currentCapacity = value; }

    }
    [SerializeField] private int _currentLoad;

    public int CurrentLoad { 
        get { return _currentLoad;}
        private set { _currentLoad = value; }
    }
    
    [SerializeField] private int _soulsSaved;

    public int SoulsSaved { 
        get { return _soulsSaved;}
        private set { _soulsSaved += value; }
    }
    
    [SerializeField] private int _soulsDamned;

    public int SoulsDamned { 
        get { return _soulsDamned;}
        private set { _soulsDamned += value; }
    }
    

    public static event Action OnBoatDestroyed;
    public static event Action OnAllSoulsLost;

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
    void Start()
    {
        ResetCapacity();
        
        //TODO Fix
        //Weird math happens here, because GameStateManager.OnFerryingEnter calls FillSouls when the game starts. True results in initial load being -Int.MaxValue.
        //CurrentLoad = doesStartLoaded ? startingCapacity : 0;
    }

    /// <summary>
    /// Increase the capacity of the boat.
    /// </summary>
    /// <param name="amount">By how many units should the maximum capacity be increased.</param>
    public void IncreaseCapacity(int amount = 1)
    {
        CurrentCapacity += amount;
    }

    public void IncreaseCapacity()
    {
        IncreaseCapacity(capacityRestoredOnSuccessfulFerry);
    }

    private int DecreaseCapacity(int amount = 1)
    {
        CurrentCapacity = CurrentCapacity - amount;
        if (CurrentCapacity == 0)
        {
            OnBoatDestroyed?.Invoke();
        }
        else if (CurrentCapacity < 0)
        {
            CurrentCapacity = 0;
        }
        return ReduceLoadToFitCapacity();
    }

    /// <summary>
    /// Reset the currentCapacity to the startingCapacity. Trim excess currentLoad to match new capacity.
    /// <returns>The number of units that were lost.</returns>
    /// </summary>
    public int ResetCapacity()
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
        // If we have no souls on the ferry, and the game is still running, we must be Returning.
        if (CurrentLoad == 0)
        {
            DecreaseCapacity(); 
        }
        // Otherwise, we must be Ferrying.
        else
        {
            //Add the Souls Lost to the SoulsDamned statistic
            SoulsDamned = DecreaseSouls(damageToTake);
            
            // If we reduce the number of souls to zero by taking damage, end the game.
            if (CurrentLoad == 0) { OnAllSoulsLost?.Invoke(); }

            if (doesLoseCapacityWhileContainsSouls)
            {
                DecreaseCapacity(damageToTake);
            }
        }
        if (CurrentCapacity == 0) { OnBoatDestroyed?.Invoke(); }
    }

    /// <summary>
    /// Add a number of units to the load of the boat. Excess units that couldn't be loaded are returned.
    /// </summary>
    /// <param name="loadToAdd">How many units are we trying to add to the load.</param>
    /// <returns>The number of units that were not able to fit onto the boat due to load capacity limits.</returns>
    public int IncreaseSouls(int loadToAdd=int.MaxValue)
    {
        CurrentLoad += loadToAdd;
        return ReduceLoadToFitCapacity();
    }

    //The set up event actions requires no return type and no parameters. This function just fulfills that requirement.
    //TODO replace with Delegates might work?
    public void FillSouls()
    {
        IncreaseSouls();
    }
    
    private int DecreaseSouls(int loadToRemove)
    {
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

    //The set up event actions requires no return type and no parameters. This function just fulfills that requirement.
    //TODO replace with Delegates might work?
    //This is run via the event Action GameStateManager.OnReturningEnter
    public void SaveAllSouls()
    {
        //Add the Souls Lost to the SoulsDamned statistic
        SoulsSaved = UnloadAll();
    }
    
    public int UnloadAll()
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
}
