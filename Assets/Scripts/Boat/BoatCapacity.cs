using System;
using UnityEngine;

public class BoatCapacity : MonoBehaviour
{
    [Tooltip("The amount of souls that can fit at the boat when the game starts.")]
    [SerializeField]
    private int startingCapacity = 6;

    [Tooltip("Does the boat start loaded to capacity? or does it start empty?")]
    [SerializeField]
    private bool doesStartLoaded = false;

    [Tooltip("Does the boat capacity get reduced when taking damage while the boat contains souls?")]
    [SerializeField]
    private bool doesLoseCapacityWhileContainsSouls = false;

    public int currentCapacity { get; private set; }
    public int currentLoad { get; private set; }

    public static event Action OnBoatDestroyed;
    public static event Action OnAllSoulsLost;

    // Start is called before the first frame update
    void Start()
    {
        ResetCapacity();
        currentLoad = doesStartLoaded ? startingCapacity : 0;
    }

    /// <summary>
    /// Increase the capacity of the boat.
    /// </summary>
    /// <param name="amount">By how many units should the maximum capacity be increased.</param>
    public void IncreaseCapacity(int amount = 1)
    {
        currentCapacity += amount;
    }

    private int DecreaseCapacity(int amount = 1)
    {
        currentCapacity = currentCapacity - amount;
        if (currentCapacity == 0)
        {
            OnBoatDestroyed?.Invoke();
        }
        else if (currentCapacity < 0)
        {
            currentCapacity = 0;
        }
        return ReduceLoadToFitCapacity();
    }

    /// <summary>
    /// Reset the currentCapacity to the startingCapacity. Trim excess currentLoad to match new capacity.
    /// <returns>The number of units that were lost.</returns>
    /// </summary>
    public int ResetCapacity()
    {
        currentCapacity = startingCapacity;
        return ReduceLoadToFitCapacity();
    }

    /// <summary>
    /// Deal damage to the boat, potentially causing the boat to trigger game-ending events.
    /// </summary>
    /// <param name="damageToTake">How many souls or how much capacity the boat should lose.</param>
    public void DealDamageToBoat(int damageToTake)
    {
        // If we have no souls on the ferry, and the game is still running, we must be Returning.
        if (currentLoad == 0)
        {
            DecreaseCapacity(); 
        }
        // Otherwise, we must be Ferrying.
        else
        {
            DecreaseSouls(damageToTake);
            // If we reduce the number of souls to zero by taking damage, end the game.
            if (currentLoad == 0) { OnAllSoulsLost?.Invoke(); }

            if (doesLoseCapacityWhileContainsSouls)
            {
                DecreaseCapacity(damageToTake);
            }
        }
        if (currentCapacity == 0) { OnBoatDestroyed?.Invoke(); }
    }

    /// <summary>
    /// Add a number of units to the load of the boat. Excess units that couldn't be loaded are returned.
    /// </summary>
    /// <param name="loadToAdd">How many units are we trying to add to the load.</param>
    /// <returns>The number of units that were not able to fit onto the boat due to load capacity limits.</returns>
    public int IncreaseSouls(int loadToAdd)
    {
        currentLoad += loadToAdd;
        return ReduceLoadToFitCapacity();
    }

    private int DecreaseSouls(int loadToRemove)
    {
        if (currentLoad <= loadToRemove)
        {

            return UnloadAll();
        }
        else
        {
            currentLoad -= loadToRemove;
            return loadToRemove;
        }
    }
    
    public int UnloadAll()
    {
        int load = currentLoad;
        currentLoad = 0;
        return load;
    }

    /// <summary>
    /// Remove excess load 
    /// </summary>
    /// <returns>The number of units that were lost.</returns>
    private int ReduceLoadToFitCapacity()
    {
        // Excess load is lost.
        if (currentLoad > currentCapacity)
        {
            int excess = currentLoad - currentCapacity;
            currentLoad = currentCapacity;
            return excess;
        }
        else
        {
            return 0;
        }

    }
}
